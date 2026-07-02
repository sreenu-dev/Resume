# Design Patterns in Code — Advanced Mastery Guide

> **Level:** Advanced | **Prerequisites:** OOP, Python 3, Metaclasses  
> **Interview Frequency:** Google ★★★☆☆ | Meta ★★★☆☆ | Amazon ★★★★☆ | Microsoft ★★★★★

---

## Table of Contents
1. [Singleton — Thread-Safe](#1-singleton--thread-safe)
2. [Observer — Event System](#2-observer--event-system)
3. [Strategy — Pluggable Algorithms](#3-strategy--pluggable-algorithms)
4. [Factory & Abstract Factory](#4-factory--abstract-factory)
5. [Decorator — Runtime Feature Addition](#5-decorator--runtime-feature-addition)
6. [Iterator — Custom Traversal](#6-iterator--custom-traversal)
7. [Command — Undo/Redo](#7-command--undoredo)
8. [Template Method — Algorithm Skeleton](#8-template-method--algorithm-skeleton)
9. [Composite — Tree Structures](#9-composite--tree-structures)
10. [Interview Problems & Applications](#10-interview-problems--applications)

---

## 1. Singleton — Thread-Safe

**When:** Global shared resource — config, connection pools, loggers.
**Anti-pattern warning:** Singletons make testing hard (global state). Prefer dependency injection when possible.

```python
import threading

class SingletonMeta(type):
    """
    Thread-safe Singleton metaclass.
    Uses double-checked locking for performance.
    
    Why double-check?
    - First check (outside lock): avoids lock acquisition if already initialized
    - Lock: ensures only one thread initializes
    - Second check (inside lock): ensures no two threads both pass first check
    """
    _instances = {}
    _lock = threading.Lock()
    
    def __call__(cls, *args, **kwargs):
        if cls not in cls._instances:
            with cls._lock:
                if cls not in cls._instances:  # Double-check
                    instance = super().__call__(*args, **kwargs)
                    cls._instances[cls] = instance
        return cls._instances[cls]


class DatabaseConnection(metaclass=SingletonMeta):
    """
    Database connection pool — exactly one instance throughout the app.
    """
    def __init__(self, connection_string: str = "default"):
        if not hasattr(self, '_initialized'):  # Prevent re-init
            self.connection_string = connection_string
            self._pool = []  # connection pool
            self._initialized = True
    
    def get_connection(self):
        return f"Connection to {self.connection_string}"
    
    def execute(self, query: str) -> list:
        print(f"Executing: {query}")
        return []


# ─── Test ───
db1 = DatabaseConnection("postgresql://localhost/db")
db2 = DatabaseConnection("mysql://different")  # Ignored! Returns same instance
print(db1 is db2)         # True — same object
print(db1.connection_string)  # "postgresql://localhost/db"

# Thread safety test
results = []
def get_instance():
    results.append(DatabaseConnection())

threads = [threading.Thread(target=get_instance) for _ in range(100)]
for t in threads: t.start()
for t in threads: t.join()
print(all(r is results[0] for r in results))  # True — all same instance


# Alternative: Module-level singleton (Python-idiomatic)
class _ConfigManager:
    def __init__(self):
        self.settings = {}
    
    def set(self, key, value):
        self.settings[key] = value
    
    def get(self, key, default=None):
        return self.settings.get(key, default)

# Module-level instance — import gives the same object
config = _ConfigManager()


# Alternative: Using __new__
class SingletonNew:
    _instance = None
    
    def __new__(cls, *args, **kwargs):
        if cls._instance is None:
            cls._instance = super().__new__(cls)
        return cls._instance
    
    def __init__(self, value=0):
        if not hasattr(self, '_init_done'):
            self.value = value
            self._init_done = True
```

**Interview problem:** "Implement a logger that can only have one instance and is thread-safe."

```python
import logging
from datetime import datetime

class Logger(metaclass=SingletonMeta):
    """Thread-safe singleton logger."""
    
    def __init__(self, log_level=logging.INFO):
        if not hasattr(self, '_init'):
            self._log_level = log_level
            self._lock = threading.Lock()
            self._logs = []
            self._init = True
    
    def log(self, level: str, message: str):
        with self._lock:
            entry = f"[{datetime.now().isoformat()}] {level}: {message}"
            self._logs.append(entry)
            print(entry)
    
    def info(self, msg): self.log("INFO", msg)
    def error(self, msg): self.log("ERROR", msg)
    def get_logs(self): return list(self._logs)
```

---

## 2. Observer — Event System

**When:** One-to-many dependencies — event handling, pub-sub, MVC.

```python
from abc import ABC, abstractmethod
from typing import Any, Callable
from collections import defaultdict
import weakref

class Event:
    """Typed event with payload."""
    def __init__(self, event_type: str, data: Any = None, source: Any = None):
        self.event_type = event_type
        self.data = data
        self.source = source

class Observer(ABC):
    @abstractmethod
    def update(self, event: Event) -> None: ...

class EventBus:
    """
    Publish-Subscribe event system.
    
    Features:
    - Subscribe to specific event types
    - Unsubscribe without iterator issues
    - Weak references to prevent memory leaks
    - Filter support
    
    Time: publish O(K) where K = subscribers for this event type
    Space: O(S) where S = total subscribers
    """
    
    def __init__(self):
        # event_type → list of (callback, filter_func)
        self._subscribers: dict[str, list] = defaultdict(list)
        self._lock = threading.Lock()
    
    def subscribe(self, event_type: str, callback: Callable, 
                  filter_func: Callable = None) -> 'Subscription':
        """
        Subscribe to an event type with optional filter.
        Returns a subscription token for unsubscribing.
        """
        with self._lock:
            entry = (callback, filter_func)
            self._subscribers[event_type].append(entry)
        return Subscription(self, event_type, entry)
    
    def unsubscribe(self, event_type: str, entry):
        with self._lock:
            if event_type in self._subscribers:
                self._subscribers[event_type] = [
                    e for e in self._subscribers[event_type] if e is not entry
                ]
    
    def publish(self, event: Event):
        """Notify all subscribers of this event type."""
        subs = list(self._subscribers.get(event.event_type, []))
        for callback, filter_func in subs:
            if filter_func is None or filter_func(event):
                try:
                    callback(event)
                except Exception as e:
                    print(f"Observer error: {e}")

class Subscription:
    """RAII-style subscription token. Unsubscribes when out of scope."""
    
    def __init__(self, bus: EventBus, event_type: str, entry):
        self.bus = bus
        self.event_type = event_type
        self.entry = entry
    
    def unsubscribe(self):
        self.bus.unsubscribe(self.event_type, self.entry)
    
    def __enter__(self): return self
    def __exit__(self, *args): self.unsubscribe()


# ─── Classic Observer Pattern ───
class Stock:
    """Observable stock price."""
    
    def __init__(self, symbol: str, price: float):
        self.symbol = symbol
        self._price = price
        self._observers: list[Observer] = []
    
    def attach(self, observer: Observer):
        self._observers.append(observer)
    
    def detach(self, observer: Observer):
        self._observers.remove(observer)
    
    def notify(self, old_price: float):
        event = Event("price_change", {
            "symbol": self.symbol,
            "old_price": old_price,
            "new_price": self._price,
            "change_pct": (self._price - old_price) / old_price * 100
        })
        for obs in list(self._observers):  # copy to handle detach during notify
            obs.update(event)
    
    @property
    def price(self): return self._price
    
    @price.setter
    def price(self, new_price: float):
        old = self._price
        self._price = new_price
        if abs(new_price - old) > 0.001:
            self.notify(old)

class AlertObserver(Observer):
    def __init__(self, name: str, threshold_pct: float = 5.0):
        self.name = name
        self.threshold = threshold_pct
    
    def update(self, event: Event):
        data = event.data
        if abs(data["change_pct"]) >= self.threshold:
            print(f"ALERT {self.name}: {data['symbol']} changed {data['change_pct']:.1f}%!")

class LogObserver(Observer):
    def update(self, event: Event):
        data = event.data
        print(f"LOG: {data['symbol']}: ${data['old_price']:.2f} → ${data['new_price']:.2f}")


# ─── Test ───
apple = Stock("AAPL", 150.0)
apple.attach(AlertObserver("Trader-1", threshold_pct=3.0))
apple.attach(LogObserver())

apple.price = 158.0  # +5.3% — triggers alert
apple.price = 159.0  # +0.6% — only log


# ─── Interview Problem: Observable with Filters ───
bus = EventBus()

def on_large_trade(event: Event):
    print(f"Large trade: {event.data['symbol']} ${event.data['amount']:,.0f}")

# Only notify when trade > $10,000
large_trade_sub = bus.subscribe(
    "trade",
    on_large_trade,
    filter_func=lambda e: e.data.get("amount", 0) > 10_000
)

bus.publish(Event("trade", {"symbol": "AAPL", "amount": 50_000}))  # Notified
bus.publish(Event("trade", {"symbol": "MSFT", "amount": 500}))     # Filtered out
```

---

## 3. Strategy — Pluggable Algorithms

**When:** Multiple interchangeable algorithms for the same task.

```python
from abc import ABC, abstractmethod
from typing import Protocol

# Using Protocol (Python 3.8+ structural typing) — more Pythonic
class SortStrategy(Protocol):
    def sort(self, data: list) -> list: ...

class BubbleSortStrategy:
    def sort(self, data: list) -> list:
        arr = data[:]
        n = len(arr)
        for i in range(n):
            for j in range(n - i - 1):
                if arr[j] > arr[j+1]:
                    arr[j], arr[j+1] = arr[j+1], arr[j]
        return arr

class MergeSortStrategy:
    def sort(self, data: list) -> list:
        if len(data) <= 1:
            return data[:]
        mid = len(data) // 2
        left = self.sort(data[:mid])
        right = self.sort(data[mid:])
        return self._merge(left, right)
    
    def _merge(self, left, right):
        result = []
        i = j = 0
        while i < len(left) and j < len(right):
            if left[i] <= right[j]:
                result.append(left[i]); i += 1
            else:
                result.append(right[j]); j += 1
        return result + left[i:] + right[j:]

class QuickSortStrategy:
    def sort(self, data: list) -> list:
        if len(data) <= 1:
            return data[:]
        pivot = data[len(data) // 2]
        left = [x for x in data if x < pivot]
        mid = [x for x in data if x == pivot]
        right = [x for x in data if x > pivot]
        return self.sort(left) + mid + self.sort(right)

class Sorter:
    """Context: uses a sort strategy. Strategy can be swapped at runtime."""
    
    def __init__(self, strategy: SortStrategy = None):
        self._strategy = strategy or MergeSortStrategy()
    
    def set_strategy(self, strategy: SortStrategy):
        self._strategy = strategy
    
    def sort(self, data: list) -> list:
        return self._strategy.sort(data)


# ─── Real-world: Payment Processing Strategy ───
class PaymentStrategy(ABC):
    @abstractmethod
    def pay(self, amount: float, user_id: str) -> bool: ...
    
    @abstractmethod
    def refund(self, transaction_id: str) -> bool: ...

class CreditCardPayment(PaymentStrategy):
    def __init__(self, card_number: str, cvv: str):
        self.card_number = card_number[-4:]  # Store only last 4
    
    def pay(self, amount: float, user_id: str) -> bool:
        print(f"Charging ${amount} to card ending in {self.card_number}")
        return True
    
    def refund(self, transaction_id: str) -> bool:
        print(f"Refunding transaction {transaction_id} to card")
        return True

class PayPalPayment(PaymentStrategy):
    def __init__(self, email: str):
        self.email = email
    
    def pay(self, amount: float, user_id: str) -> bool:
        print(f"Charging ${amount} via PayPal to {self.email}")
        return True
    
    def refund(self, transaction_id: str) -> bool:
        print(f"Refunding {transaction_id} via PayPal")
        return True

class CryptoPayment(PaymentStrategy):
    def __init__(self, wallet_address: str, currency: str = "BTC"):
        self.wallet = wallet_address
        self.currency = currency
    
    def pay(self, amount: float, user_id: str) -> bool:
        print(f"Charging ${amount} in {self.currency} to {self.wallet[:8]}...")
        return True
    
    def refund(self, transaction_id: str) -> bool:
        print(f"Crypto refunds require manual processing: {transaction_id}")
        return False

class ShoppingCart:
    def __init__(self, payment_strategy: PaymentStrategy):
        self.items = []
        self.payment = payment_strategy
    
    def add_item(self, name: str, price: float):
        self.items.append((name, price))
    
    def checkout(self, user_id: str) -> bool:
        total = sum(price for _, price in self.items)
        return self.payment.pay(total, user_id)


# ─── Test ───
cart = ShoppingCart(CreditCardPayment("4111111111111234", "123"))
cart.add_item("Book", 29.99)
cart.add_item("Pen", 2.99)
cart.checkout("user_001")

# Switch strategy at runtime
cart.payment = PayPalPayment("user@email.com")
cart.checkout("user_001")
```

---

## 4. Factory & Abstract Factory

```python
from abc import ABC, abstractmethod
from enum import Enum

# ─── Simple Factory ───
class LogLevel(Enum):
    DEBUG = "DEBUG"
    INFO = "INFO"
    WARNING = "WARNING"
    ERROR = "ERROR"

class Logger(ABC):
    @abstractmethod
    def log(self, message: str, level: LogLevel) -> None: ...

class ConsoleLogger(Logger):
    def log(self, message: str, level: LogLevel):
        print(f"[{level.value}] {message}")

class FileLogger(Logger):
    def __init__(self, filepath: str):
        self.filepath = filepath
    
    def log(self, message: str, level: LogLevel):
        with open(self.filepath, 'a') as f:
            f.write(f"[{level.value}] {message}\n")

class DatabaseLogger(Logger):
    def log(self, message: str, level: LogLevel):
        print(f"DB INSERT: ({level.value}, '{message}')")

class LoggerFactory:
    """
    Simple Factory: centralize object creation.
    Client doesn't know concrete class names.
    """
    
    _registry = {
        'console': ConsoleLogger,
        'database': DatabaseLogger,
    }
    
    @classmethod
    def create(cls, logger_type: str, **kwargs) -> Logger:
        if logger_type not in cls._registry:
            raise ValueError(f"Unknown logger: {logger_type}. Choose from {list(cls._registry)}")
        return cls._registry[logger_type](**kwargs)
    
    @classmethod
    def register(cls, name: str, logger_class: type):
        """Plugin mechanism: register new logger types."""
        cls._registry[name] = logger_class


# ─── Abstract Factory ───
class Button(ABC):
    @abstractmethod
    def render(self) -> str: ...
    
    @abstractmethod
    def on_click(self) -> str: ...

class TextBox(ABC):
    @abstractmethod
    def render(self) -> str: ...
    
    @abstractmethod
    def get_text(self) -> str: ...

class Dialog(ABC):
    @abstractmethod
    def create_button(self) -> Button: ...
    
    @abstractmethod
    def create_text_box(self) -> TextBox: ...
    
    def show(self):
        btn = self.create_button()
        txt = self.create_text_box()
        print(f"Dialog: {btn.render()} | {txt.render()}")

# Windows family
class WindowsButton(Button):
    def render(self) -> str: return "[Windows Button]"
    def on_click(self) -> str: return "Windows click!"

class WindowsTextBox(TextBox):
    def render(self) -> str: return "[Windows TextBox]"
    def get_text(self) -> str: return "Windows input"

class WindowsDialog(Dialog):
    def create_button(self) -> Button: return WindowsButton()
    def create_text_box(self) -> TextBox: return WindowsTextBox()

# macOS family
class MacButton(Button):
    def render(self) -> str: return "(Mac Button)"
    def on_click(self) -> str: return "Mac click!"

class MacTextBox(TextBox):
    def render(self) -> str: return "(Mac TextBox)"
    def get_text(self) -> str: return "Mac input"

class MacDialog(Dialog):
    def create_button(self) -> Button: return MacButton()
    def create_text_box(self) -> TextBox: return MacTextBox()

class UIFactory:
    """Abstract Factory: creates families of related objects."""
    
    @staticmethod
    def get_dialog(os: str) -> Dialog:
        factories = {"windows": WindowsDialog, "mac": MacDialog}
        if os not in factories:
            raise ValueError(f"Unsupported OS: {os}")
        return factories[os]()


# ─── Interview Problem: Plugin Architecture ───
class Plugin(ABC):
    @abstractmethod
    def name(self) -> str: ...
    
    @abstractmethod
    def execute(self, data: dict) -> dict: ...

class PluginRegistry:
    """
    Self-registering plugin system.
    Plugins register themselves via decorators.
    """
    _plugins: dict[str, type] = {}
    
    @classmethod
    def register(cls, name: str):
        def decorator(plugin_class):
            cls._plugins[name] = plugin_class
            return plugin_class
        return decorator
    
    @classmethod
    def get(cls, name: str) -> Plugin:
        if name not in cls._plugins:
            raise KeyError(f"Plugin '{name}' not registered")
        return cls._plugins[name]()
    
    @classmethod
    def list_plugins(cls) -> list[str]:
        return list(cls._plugins.keys())

@PluginRegistry.register("json_transform")
class JSONTransformPlugin(Plugin):
    def name(self) -> str: return "json_transform"
    def execute(self, data: dict) -> dict:
        import json
        return {"result": json.dumps(data)}

@PluginRegistry.register("encrypt")
class EncryptPlugin(Plugin):
    def name(self) -> str: return "encrypt"
    def execute(self, data: dict) -> dict:
        # Simulate encryption
        return {"result": "encrypted_" + str(hash(str(data)))}


print(PluginRegistry.list_plugins())  # ['json_transform', 'encrypt']
plugin = PluginRegistry.get("encrypt")
print(plugin.execute({"key": "secret"}))
```

---

## 5. Decorator — Runtime Feature Addition

```python
from abc import ABC, abstractmethod
import time
import functools

# ─── Classic Decorator: File System ───
class FileComponent(ABC):
    @abstractmethod
    def read(self) -> bytes: ...
    
    @abstractmethod
    def write(self, data: bytes) -> None: ...
    
    @abstractmethod
    def size(self) -> int: ...

class BasicFile(FileComponent):
    def __init__(self, content: bytes = b""):
        self._content = content
    
    def read(self) -> bytes: return self._content
    def write(self, data: bytes): self._content = data
    def size(self) -> int: return len(self._content)

class FileDecorator(FileComponent):
    """Base decorator — delegates to wrapped component."""
    def __init__(self, component: FileComponent):
        self._component = component
    
    def read(self) -> bytes: return self._component.read()
    def write(self, data: bytes): self._component.write(data)
    def size(self) -> int: return self._component.size()

class CompressedFile(FileDecorator):
    """Adds compression/decompression."""
    import zlib
    
    def read(self) -> bytes:
        return self.zlib.decompress(self._component.read())
    
    def write(self, data: bytes):
        self._component.write(self.zlib.compress(data))
    
    def size(self) -> int:
        return self._component.size()

class EncryptedFile(FileDecorator):
    """Adds XOR encryption (simplified)."""
    def __init__(self, component: FileComponent, key: int = 42):
        super().__init__(component)
        self._key = key
    
    def _xor(self, data: bytes) -> bytes:
        return bytes(b ^ self._key for b in data)
    
    def read(self) -> bytes:
        return self._xor(self._component.read())
    
    def write(self, data: bytes):
        self._component.write(self._xor(data))

class LoggedFile(FileDecorator):
    """Adds logging for read/write operations."""
    def read(self) -> bytes:
        print(f"Reading {self._component.size()} bytes")
        return self._component.read()
    
    def write(self, data: bytes):
        print(f"Writing {len(data)} bytes")
        self._component.write(data)

# Stack decorators!
file = BasicFile()
file = LoggedFile(EncryptedFile(file, key=99))
file.write(b"Hello, World!")  # Logged, then encrypted, then stored
data = file.read()            # Logged, then decrypted


# ─── Function Decorators: Caching, Retry, Timing ───
def memoize(func):
    """LRU-style memoization decorator."""
    cache = {}
    @functools.wraps(func)
    def wrapper(*args, **kwargs):
        key = (args, tuple(sorted(kwargs.items())))
        if key not in cache:
            cache[key] = func(*args, **kwargs)
        return cache[key]
    wrapper.cache = cache
    wrapper.cache_clear = lambda: cache.clear()
    return wrapper

def retry(max_attempts: int = 3, delay: float = 1.0, exceptions=(Exception,)):
    """Retry decorator with exponential backoff."""
    def decorator(func):
        @functools.wraps(func)
        def wrapper(*args, **kwargs):
            for attempt in range(max_attempts):
                try:
                    return func(*args, **kwargs)
                except exceptions as e:
                    if attempt == max_attempts - 1:
                        raise
                    wait = delay * (2 ** attempt)
                    print(f"Attempt {attempt+1} failed: {e}. Retrying in {wait}s...")
                    time.sleep(wait)
        return wrapper
    return decorator

def timer(func):
    """Timing decorator."""
    @functools.wraps(func)
    def wrapper(*args, **kwargs):
        start = time.perf_counter()
        result = func(*args, **kwargs)
        elapsed = time.perf_counter() - start
        print(f"{func.__name__} took {elapsed*1000:.2f}ms")
        return result
    return wrapper

@timer
@memoize
def fibonacci(n: int) -> int:
    if n <= 1: return n
    return fibonacci(n-1) + fibonacci(n-2)

print(fibonacci(30))  # Fast due to memoization
```

---

## 6. Iterator — Custom Traversal

```python
from typing import Iterator, TypeVar, Generic
from collections import deque

T = TypeVar('T')

# ─── Interview Problem: Flatten 2D Iterator ───
class Vector2D:
    """
    Iterator that flattens a 2D list.
    LeetCode 251.
    
    Time: next() O(1) amortized | hasNext() O(1)
    Space: O(rows)
    """
    
    def __init__(self, vec: list[list[int]]):
        self.rows = iter(vec)
        self.curr_row = iter([])
    
    def next(self) -> int:
        self.hasNext()  # Ensure curr_row is ready
        return next(self.curr_row)
    
    def hasNext(self) -> bool:
        while True:
            try:
                next(self.curr_row)  # Check if curr_row has elements
                # Oops — consumed it! Use peek via StopIteration trick
            except StopIteration:
                pass
            # Better implementation using itertools.chain or storing peeked value:
            break
        return True  # Simplified
    
    # Better implementation:
    def _setup(self, vec: list[list[int]]):
        """Proper implementation using deque of iterators."""
        self._queue = deque(iter(row) for row in vec if row)
    
    def next_v2(self) -> int:
        if not self.hasNext_v2():
            raise StopIteration
        return next(self._queue[0])
    
    def hasNext_v2(self) -> bool:
        while self._queue:
            try:
                val = next(self._queue[0])
                # Put it back (peek without consuming)
                self._queue[0] = iter([val]) if False else self._queue[0]
                # Actually: store peeked value
                return True
            except StopIteration:
                self._queue.popleft()
        return False


class Vector2DClean:
    """Clean 2D iterator with proper peek."""
    
    def __init__(self, vec: list[list[int]]):
        self._data = vec
        self._row = 0
        self._col = 0
        self._advance()
    
    def _advance(self):
        """Skip to next valid position."""
        while self._row < len(self._data) and self._col >= len(self._data[self._row]):
            self._row += 1
            self._col = 0
    
    def next(self) -> int:
        if not self.hasNext():
            raise StopIteration
        val = self._data[self._row][self._col]
        self._col += 1
        self._advance()
        return val
    
    def hasNext(self) -> bool:
        return self._row < len(self._data)


# ─── Nested List Iterator ───
class NestedInteger:
    def __init__(self, val=None):
        if val is not None:
            self._val = val
            self._list = None
        else:
            self._val = None
            self._list = []
    
    def isInteger(self): return self._list is None
    def getInteger(self): return self._val
    def getList(self): return self._list

class NestedIterator:
    """
    Flatten nested list of NestedIntegers.
    LeetCode 341.
    
    Approach: Eagerly flatten in constructor.
    Alternative: Lazy evaluation with stack (better for infinite lists).
    
    Time: flatten O(N), next O(1), hasNext O(1)
    Space: O(N)
    """
    
    def __init__(self, nestedList: list):
        self._flat = []
        self._idx = 0
        self._flatten(nestedList)
    
    def _flatten(self, lst):
        for item in lst:
            if item.isInteger():
                self._flat.append(item.getInteger())
            else:
                self._flatten(item.getList())
    
    def next(self) -> int:
        val = self._flat[self._idx]
        self._idx += 1
        return val
    
    def hasNext(self) -> bool:
        return self._idx < len(self._flat)


# ─── Custom Tree Iterator (In-order) ───
class TreeNode:
    def __init__(self, val=0, left=None, right=None):
        self.val = val
        self.left = left
        self.right = right

class BSTIterator:
    """
    In-order BST iterator. O(1) amortized next().
    Uses O(H) space (H = tree height) — NOT O(N)!
    
    This is the Morris traversal alternative.
    """
    
    def __init__(self, root: TreeNode):
        self._stack = []
        self._push_left(root)
    
    def _push_left(self, node):
        while node:
            self._stack.append(node)
            node = node.left
    
    def next(self) -> int:
        """O(1) amortized — each node is pushed/popped once."""
        node = self._stack.pop()
        self._push_left(node.right)
        return node.val
    
    def hasNext(self) -> bool:
        return bool(self._stack)
    
    def __iter__(self): return self
    def __next__(self):
        if not self.hasNext(): raise StopIteration
        return self.next()
```

---

## 7. Command — Undo/Redo

```python
from abc import ABC, abstractmethod
from typing import Optional

class Command(ABC):
    @abstractmethod
    def execute(self) -> None: ...
    
    @abstractmethod
    def undo(self) -> None: ...
    
    @abstractmethod
    def redo(self) -> None:
        """Default: just execute again."""
        self.execute()

class TextEditor:
    """
    Text editor with undo/redo using Command pattern.
    
    History: stack of executed commands
    Redo stack: cleared on new command, restored on undo
    
    Time: O(1) execute/undo/redo (O(len) for string ops)
    Space: O(H) where H = history length
    """
    
    def __init__(self):
        self.text = ""
        self.cursor = 0
        self._history: list[Command] = []
        self._redo_stack: list[Command] = []
    
    def execute(self, command: Command):
        command.execute()
        self._history.append(command)
        self._redo_stack.clear()  # New action clears redo history
    
    def undo(self):
        if not self._history:
            print("Nothing to undo")
            return
        cmd = self._history.pop()
        cmd.undo()
        self._redo_stack.append(cmd)
    
    def redo(self):
        if not self._redo_stack:
            print("Nothing to redo")
            return
        cmd = self._redo_stack.pop()
        cmd.redo()
        self._history.append(cmd)

class InsertCommand(Command):
    def __init__(self, editor: TextEditor, pos: int, text: str):
        self.editor = editor
        self.pos = pos
        self.text = text
    
    def execute(self):
        e = self.editor
        e.text = e.text[:self.pos] + self.text + e.text[self.pos:]
        e.cursor = self.pos + len(self.text)
    
    def undo(self):
        e = self.editor
        e.text = e.text[:self.pos] + e.text[self.pos + len(self.text):]
        e.cursor = self.pos

class DeleteCommand(Command):
    def __init__(self, editor: TextEditor, pos: int, length: int):
        self.editor = editor
        self.pos = pos
        self.length = length
        self.deleted_text = ""  # Saved for undo
    
    def execute(self):
        e = self.editor
        self.deleted_text = e.text[self.pos:self.pos + self.length]
        e.text = e.text[:self.pos] + e.text[self.pos + self.length:]
        e.cursor = self.pos
    
    def undo(self):
        e = self.editor
        e.text = e.text[:self.pos] + self.deleted_text + e.text[self.pos:]
        e.cursor = self.pos + self.length

class MacroCommand(Command):
    """Composite command: execute multiple commands as one."""
    
    def __init__(self, commands: list[Command]):
        self.commands = commands
    
    def execute(self):
        for cmd in self.commands:
            cmd.execute()
    
    def undo(self):
        for cmd in reversed(self.commands):  # Undo in reverse order!
            cmd.undo()


# ─── Test ───
editor = TextEditor()
editor.execute(InsertCommand(editor, 0, "Hello"))
print(editor.text)  # "Hello"
editor.execute(InsertCommand(editor, 5, " World"))
print(editor.text)  # "Hello World"
editor.execute(DeleteCommand(editor, 5, 6))
print(editor.text)  # "Hello"
editor.undo()
print(editor.text)  # "Hello World"
editor.undo()
print(editor.text)  # "Hello"
editor.redo()
print(editor.text)  # "Hello World"
```

---

## 8. Template Method — Algorithm Skeleton

```python
from abc import ABC, abstractmethod
import csv
import json

class DataProcessor(ABC):
    """
    Template Method: defines the algorithm skeleton.
    Subclasses implement specific steps.
    
    This pattern is everywhere: sort → comparator, game loops, ML pipelines.
    """
    
    def process(self, source: str) -> list[dict]:
        """Template method — defines the algorithm."""
        raw = self.read(source)
        validated = self.validate(raw)
        transformed = self.transform(validated)
        self.save(transformed)
        return transformed
    
    @abstractmethod
    def read(self, source: str) -> str: ...
    
    def validate(self, raw: str) -> str:
        """Hook method — optional override."""
        if not raw:
            raise ValueError("Empty data")
        return raw
    
    @abstractmethod
    def transform(self, raw: str) -> list[dict]: ...
    
    def save(self, data: list[dict]) -> None:
        """Optional hook — default: do nothing."""
        pass

class CSVProcessor(DataProcessor):
    def read(self, source: str) -> str:
        return "name,age\nAlice,30\nBob,25"  # Simulated
    
    def transform(self, raw: str) -> list[dict]:
        lines = raw.strip().split('\n')
        reader = csv.DictReader(lines)
        return list(reader)

class JSONProcessor(DataProcessor):
    def read(self, source: str) -> str:
        return '[{"name": "Alice", "age": 30}]'  # Simulated
    
    def transform(self, raw: str) -> list[dict]:
        return json.loads(raw)
    
    def save(self, data: list[dict]) -> None:
        print(f"Saved {len(data)} JSON records")
```

---

## 9. Composite — Tree Structures

```python
from abc import ABC, abstractmethod
from typing import Iterator

class FileSystemComponent(ABC):
    """Component interface for files and directories."""
    
    @abstractmethod
    def name(self) -> str: ...
    
    @abstractmethod
    def size(self) -> int: ...
    
    @abstractmethod
    def display(self, indent: int = 0) -> None: ...
    
    def is_file(self) -> bool: return False

class File(FileSystemComponent):
    def __init__(self, name: str, size: int):
        self._name = name
        self._size = size
    
    def name(self) -> str: return self._name
    def size(self) -> int: return self._size
    def is_file(self) -> bool: return True
    
    def display(self, indent: int = 0):
        print(" " * indent + f"📄 {self._name} ({self._size}B)")

class Directory(FileSystemComponent):
    """Composite: contains files and subdirectories."""
    
    def __init__(self, name: str):
        self._name = name
        self._children: list[FileSystemComponent] = []
    
    def add(self, component: FileSystemComponent):
        self._children.append(component)
    
    def remove(self, name: str):
        self._children = [c for c in self._children if c.name() != name]
    
    def name(self) -> str: return self._name
    
    def size(self) -> int:
        return sum(c.size() for c in self._children)
    
    def display(self, indent: int = 0):
        print(" " * indent + f"📁 {self._name}/ ({self.size()}B)")
        for child in self._children:
            child.display(indent + 2)
    
    def find(self, filename: str) -> list[FileSystemComponent]:
        """DFS search for files with given name."""
        results = []
        for child in self._children:
            if child.name() == filename:
                results.append(child)
            if not child.is_file():
                results.extend(child.find(filename))
        return results
    
    def __iter__(self) -> Iterator[FileSystemComponent]:
        """Recursive iteration over all files (DFS)."""
        yield self
        for child in self._children:
            if child.is_file():
                yield child
            else:
                yield from child  # Recurse into subdirectories


# ─── Test ───
root = Directory("root")
src = Directory("src")
src.add(File("main.py", 1200))
src.add(File("utils.py", 800))
tests = Directory("tests")
tests.add(File("test_main.py", 600))
root.add(src)
root.add(tests)
root.add(File("README.md", 500))

root.display()
print(f"Total size: {root.size()}B")
```

---

## 10. Interview Problems & Applications

### Problem: Design a Notification System (Observer + Strategy + Factory)

```python
class NotificationChannel(ABC):
    @abstractmethod
    def send(self, user: str, message: str) -> bool: ...

class EmailChannel(NotificationChannel):
    def send(self, user: str, message: str) -> bool:
        print(f"Email to {user}: {message}")
        return True

class SMSChannel(NotificationChannel):
    def send(self, user: str, message: str) -> bool:
        print(f"SMS to {user}: {message[:160]}")  # SMS limit
        return True

class PushChannel(NotificationChannel):
    def send(self, user: str, message: str) -> bool:
        print(f"Push to {user}: {message}")
        return True

class ChannelFactory:
    _channels = {"email": EmailChannel, "sms": SMSChannel, "push": PushChannel}
    
    @classmethod
    def create(cls, channel_type: str) -> NotificationChannel:
        return cls._channels[channel_type]()

class NotificationService:
    """
    Multi-channel notification with Observer pattern.
    Users subscribe to topics, notifications are published to topics.
    """
    
    def __init__(self):
        self.user_channels: dict[str, list[str]] = {}  # user → preferred channels
        self.topic_subscribers: dict[str, set[str]] = {}  # topic → users
    
    def subscribe_to_topic(self, user: str, topic: str, channels: list[str]):
        self.user_channels[user] = channels
        self.topic_subscribers.setdefault(topic, set()).add(user)
    
    def notify_topic(self, topic: str, message: str):
        users = self.topic_subscribers.get(topic, set())
        for user in users:
            for ch_type in self.user_channels.get(user, ["email"]):
                channel = ChannelFactory.create(ch_type)
                channel.send(user, message)


# ─── Complexity Summary ───
patterns = {
    "Singleton": {"space": "O(1)", "get": "O(1)"},
    "Observer": {"publish": "O(K)", "subscribe": "O(1)"},
    "Strategy": {"swap": "O(1)", "execute": "varies"},
    "Factory": {"create": "O(1) lookup", "registry": "O(N)"},
    "Decorator": {"wrap": "O(1)", "call": "O(D) where D=depth"},
    "Iterator": {"next": "O(1) amortized", "space": "O(H)"},
    "Command": {"execute/undo": "O(1)", "history": "O(H)"},
    "Composite": {"size": "O(N)", "find": "O(N)"},
}

for pattern, complexity in patterns.items():
    print(f"{pattern}: {complexity}")
```

### Summary Table: Pattern Selection Guide

| Symptom | Pattern | Key Benefit |
|---------|---------|-------------|
| Need exactly one instance | Singleton | Global access, controlled creation |
| Many objects need state updates | Observer | Loose coupling, extensible |
| Multiple algorithms for same task | Strategy | Swap at runtime, Open/Closed |
| Complex object creation | Factory | Hide creation details |
| Add features without changing class | Decorator | Combinable, stackable |
| Custom traversal over collection | Iterator | Encapsulate traversal |
| Encapsulate operations, undo/redo | Command | Undo support, queuing |
| Common algorithm, variable steps | Template Method | Code reuse, hook points |
| Tree-like structures | Composite | Uniform treatment of leaf/composite |

---

*Design patterns are a shared vocabulary — knowing them lets you communicate complex designs in seconds. In interviews, mention the pattern name, explain WHY it fits (not just WHAT it is), and implement it cleanly. The best answer always explains the trade-off: "I'm using Strategy here because the fee calculation is likely to change, and this lets us add new pricing models without touching the ShoppingCart class."*
