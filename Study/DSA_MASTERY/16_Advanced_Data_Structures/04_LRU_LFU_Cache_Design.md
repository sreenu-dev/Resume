# LRU, LFU Cache & System Design — Advanced Mastery Guide

> **Level:** Advanced | **Prerequisites:** Doubly Linked Lists, HashMaps, OOP  
> **Interview Frequency:** Google ★★★★★ | Meta ★★★★★ | Amazon ★★★★★ | Microsoft ★★★★★

---

## Table of Contents
1. [LRU Cache — O(1) Get and Put](#1-lru-cache--o1-get-and-put)
2. [LFU Cache — O(1) Get and Put](#2-lfu-cache--o1-get-and-put)
3. [Design HashMap from Scratch](#3-design-hashmap-from-scratch)
4. [Design HashSet](#4-design-hashset)
5. [Design Twitter — Top 10 Feed](#5-design-twitter--top-10-feed)
6. [Time-Based Key-Value Store](#6-time-based-key-value-store)
7. [All-O(1) Data Structure](#7-allo1-data-structure)
8. [Design Phone Directory](#8-design-phone-directory)
9. [Random Pick Index — Reservoir Sampling](#9-random-pick-index--reservoir-sampling)
10. [Design Rate Limiter](#10-design-rate-limiter)
11. [Interview Tips & Edge Cases](#11-interview-tips--edge-cases)

---

## 1. LRU Cache — O(1) Get and Put

**Strategy:** Doubly linked list (order of recency) + HashMap (O(1) access).
The sentinel head/tail trick eliminates null checks.

```python
class DLinkedNode:
    """Doubly linked list node with key and value."""
    __slots__ = ['key', 'val', 'prev', 'next']
    
    def __init__(self, key: int = 0, val: int = 0):
        self.key = key
        self.val = val
        self.prev = None
        self.next = None


class LRUCache:
    """
    LRU Cache with O(1) get and put.
    
    Data structures:
    - HashMap: key → DLinkedNode (O(1) lookup)
    - Doubly Linked List: maintains recency order
      head.next = most recently used
      tail.prev = least recently used (eviction candidate)
    
    Sentinel nodes: head and tail never store real data,
    eliminating all null/boundary checks.
    
    Time: O(1) get, O(1) put
    Space: O(capacity)
    """
    
    def __init__(self, capacity: int):
        if capacity <= 0:
            raise ValueError("Capacity must be positive")
        self.capacity = capacity
        self.cache = {}  # key → node
        self.size = 0
        
        # Sentinel nodes: head ↔ [MRU ... LRU] ↔ tail
        self.head = DLinkedNode()  # "most recent" sentinel
        self.tail = DLinkedNode()  # "least recent" sentinel
        self.head.next = self.tail
        self.tail.prev = self.head
    
    def _remove(self, node: DLinkedNode):
        """Remove a node from the doubly linked list. O(1)."""
        node.prev.next = node.next
        node.next.prev = node.prev
    
    def _add_to_front(self, node: DLinkedNode):
        """Add node right after head (most recently used position). O(1)."""
        node.prev = self.head
        node.next = self.head.next
        self.head.next.prev = node
        self.head.next = node
    
    def _move_to_front(self, node: DLinkedNode):
        """Move existing node to front (mark as recently used). O(1)."""
        self._remove(node)
        self._add_to_front(node)
    
    def get(self, key: int) -> int:
        """
        Get value of key. Returns -1 if not found.
        Also marks key as recently used.
        Time: O(1)
        """
        if key not in self.cache:
            return -1
        node = self.cache[key]
        self._move_to_front(node)
        return node.val
    
    def put(self, key: int, value: int) -> None:
        """
        Insert or update key-value pair.
        If at capacity, evict LRU item first.
        Time: O(1)
        """
        if key in self.cache:
            node = self.cache[key]
            node.val = value
            self._move_to_front(node)
        else:
            if self.size == self.capacity:
                # Evict LRU: node before tail sentinel
                lru = self.tail.prev
                self._remove(lru)
                del self.cache[lru.key]
                self.size -= 1
            
            node = DLinkedNode(key, value)
            self.cache[key] = node
            self._add_to_front(node)
            self.size += 1
    
    def __repr__(self):
        """Show cache state from MRU to LRU."""
        result = []
        cur = self.head.next
        while cur != self.tail:
            result.append(f"{cur.key}:{cur.val}")
            cur = cur.next
        return "LRU→[" + ", ".join(result) + "]→LRU_end"


# ─── Test ───
cache = LRUCache(2)
cache.put(1, 1)
cache.put(2, 2)
print(cache.get(1))    # 1 (also moves 1 to front)
cache.put(3, 3)        # evicts 2 (LRU)
print(cache.get(2))    # -1 (evicted)
cache.put(4, 4)        # evicts 1 (LRU — 3 is MRU, 1 became LRU after 3 inserted)
print(cache.get(1))    # -1 (evicted)
print(cache.get(3))    # 3
print(cache.get(4))    # 4
```

**Complexity:** O(1) get, O(1) put | Space O(capacity)

### LRU with `OrderedDict` (Python shortcut — know but don't use in interview)

```python
from collections import OrderedDict

class LRUCacheSimple:
    """Pythonic LRU using OrderedDict. NOT suitable for interview (masks internals)."""
    
    def __init__(self, capacity: int):
        self.capacity = capacity
        self.cache = OrderedDict()
    
    def get(self, key: int) -> int:
        if key not in self.cache:
            return -1
        self.cache.move_to_end(key)  # mark as recently used
        return self.cache[key]
    
    def put(self, key: int, value: int) -> None:
        if key in self.cache:
            self.cache.move_to_end(key)
        self.cache[key] = value
        if len(self.cache) > self.capacity:
            self.cache.popitem(last=False)  # remove LRU (first item)
```

---

## 2. LFU Cache — O(1) Get and Put

**Strategy:** Three-level structure:
1. `key_map`: key → (value, frequency)
2. `freq_map`: frequency → doubly linked list of keys (ordered by recency within freq)
3. `min_freq`: current minimum frequency for O(1) eviction

```python
class LFUNode:
    __slots__ = ['key', 'val', 'freq', 'prev', 'next']
    def __init__(self, key=0, val=0, freq=1):
        self.key = key
        self.val = val
        self.freq = freq
        self.prev = self.next = None


class FreqList:
    """Doubly linked list for a single frequency bucket."""
    
    def __init__(self):
        self.head = LFUNode()  # sentinel (MRU end)
        self.tail = LFUNode()  # sentinel (LRU end)
        self.head.next = self.tail
        self.tail.prev = self.head
        self.size = 0
    
    def add_to_front(self, node: LFUNode):
        node.prev = self.head
        node.next = self.head.next
        self.head.next.prev = node
        self.head.next = node
        self.size += 1
    
    def remove(self, node: LFUNode):
        node.prev.next = node.next
        node.next.prev = node.prev
        self.size -= 1
    
    def remove_lru(self) -> LFUNode:
        """Remove and return the LRU node (before tail sentinel)."""
        if self.size == 0:
            return None
        lru = self.tail.prev
        self.remove(lru)
        return lru
    
    def is_empty(self) -> bool:
        return self.size == 0


class LFUCache:
    """
    LFU Cache with O(1) get and put.
    
    Eviction policy: evict the LEAST FREQUENTLY USED item.
    Tie-break: among equal frequencies, evict LEAST RECENTLY USED.
    
    Data structures:
    - key_map: key → node (contains value and frequency)
    - freq_map: frequency → FreqList (doubly linked list of nodes with this freq)
    - min_freq: current minimum frequency (for O(1) eviction)
    
    Key insight for O(1):
    - When we increment a node's frequency from f to f+1:
      → Remove from freq_map[f], add to freq_map[f+1]
      → If freq_map[f] becomes empty AND f == min_freq: min_freq += 1
    - When inserting new node:
      → freq = 1, min_freq = 1 (new node always has lowest freq)
    
    Time: O(1) get, O(1) put
    Space: O(capacity)
    """
    
    def __init__(self, capacity: int):
        self.capacity = capacity
        self.size = 0
        self.min_freq = 0
        self.key_map = {}      # key → LFUNode
        self.freq_map = {}     # freq → FreqList
    
    def _get_or_create_freq_list(self, freq: int) -> FreqList:
        if freq not in self.freq_map:
            self.freq_map[freq] = FreqList()
        return self.freq_map[freq]
    
    def _increment_freq(self, node: LFUNode):
        """Move node from its current frequency bucket to freq+1 bucket."""
        old_freq = node.freq
        new_freq = old_freq + 1
        
        # Remove from old frequency list
        old_list = self.freq_map[old_freq]
        old_list.remove(node)
        
        # Update min_freq if old bucket is now empty
        if old_list.is_empty() and old_freq == self.min_freq:
            self.min_freq = new_freq
        
        # Add to new frequency list (at front = most recent)
        node.freq = new_freq
        new_list = self._get_or_create_freq_list(new_freq)
        new_list.add_to_front(node)
    
    def get(self, key: int) -> int:
        """Get value of key. Returns -1 if not found. O(1)."""
        if key not in self.key_map:
            return -1
        node = self.key_map[key]
        self._increment_freq(node)
        return node.val
    
    def put(self, key: int, value: int) -> None:
        """Insert or update key-value. Evicts LFU (then LRU) if full. O(1)."""
        if self.capacity <= 0:
            return
        
        if key in self.key_map:
            # Update existing key
            node = self.key_map[key]
            node.val = value
            self._increment_freq(node)
            return
        
        # Evict if at capacity
        if self.size == self.capacity:
            evict_list = self.freq_map[self.min_freq]
            evicted = evict_list.remove_lru()
            if evicted:
                del self.key_map[evicted.key]
                self.size -= 1
        
        # Insert new node
        node = LFUNode(key, value, freq=1)
        self.key_map[key] = node
        freq1_list = self._get_or_create_freq_list(1)
        freq1_list.add_to_front(node)
        self.min_freq = 1  # New node always has freq=1 = new minimum
        self.size += 1


# ─── Test ───
lfu = LFUCache(2)
lfu.put(1, 1)    # cache: {1:freq=1}
lfu.put(2, 2)    # cache: {1:freq=1, 2:freq=1}
print(lfu.get(1))  # 1 → freq[1]=2
lfu.put(3, 3)    # evict 2 (freq=1, LRU among freq=1), cache: {1:freq=2, 3:freq=1}
print(lfu.get(2))  # -1 (evicted)
print(lfu.get(3))  # 3 → freq[3]=2
lfu.put(4, 4)    # evict 1? No: freq[1]=2, freq[3]=2, both equal → evict LRU
print(lfu.get(1))  # depends on recency: 1 was accessed before 3
print(lfu.get(3))  # 3
print(lfu.get(4))  # 4
```

**Complexity:** O(1) get, O(1) put | Space O(capacity + unique frequencies)

---

## 3. Design HashMap from Scratch

```python
class MyHashMap:
    """
    HashMap from scratch using chaining (linked list per bucket).
    
    Hash function: key % bucket_count
    Load factor resize: when size > 0.75 * bucket_count
    
    Average: O(1) put, get, remove
    Worst (all in one bucket): O(N)
    Space: O(N)
    """
    
    INITIAL_SIZE = 1024
    LOAD_FACTOR = 0.75
    
    def __init__(self):
        self.size = 0
        self.bucket_count = self.INITIAL_SIZE
        self.buckets = [[] for _ in range(self.bucket_count)]
    
    def _hash(self, key: int) -> int:
        return key % self.bucket_count
    
    def put(self, key: int, value: int) -> None:
        h = self._hash(key)
        bucket = self.buckets[h]
        for i, (k, v) in enumerate(bucket):
            if k == key:
                bucket[i] = (key, value)  # update
                return
        bucket.append((key, value))  # insert
        self.size += 1
        
        # Resize if load factor exceeded
        if self.size > self.LOAD_FACTOR * self.bucket_count:
            self._resize()
    
    def get(self, key: int) -> int:
        h = self._hash(key)
        for k, v in self.buckets[h]:
            if k == key:
                return v
        return -1
    
    def remove(self, key: int) -> None:
        h = self._hash(key)
        bucket = self.buckets[h]
        for i, (k, v) in enumerate(bucket):
            if k == key:
                bucket.pop(i)
                self.size -= 1
                return
    
    def _resize(self):
        """Double bucket count and rehash all entries."""
        old_buckets = self.buckets
        self.bucket_count *= 2
        self.buckets = [[] for _ in range(self.bucket_count)]
        self.size = 0
        for bucket in old_buckets:
            for k, v in bucket:
                self.put(k, v)


# Open Addressing (linear probing) alternative:
class MyHashMapOpenAddressing:
    """
    HashMap with open addressing (linear probing).
    Better cache performance than chaining.
    
    Average: O(1) put, get, remove
    Space: O(N) with good load factor
    """
    
    _DELETED = object()  # sentinel for deleted slots
    
    def __init__(self, size: int = 1024):
        self.size = size
        self.keys = [None] * size
        self.vals = [None] * size
        self.count = 0
    
    def _hash(self, key: int) -> int:
        return key % self.size
    
    def put(self, key: int, value: int) -> None:
        h = self._hash(key)
        i = h
        first_deleted = -1
        while self.keys[i] is not None:
            if self.keys[i] is self._DELETED:
                if first_deleted == -1:
                    first_deleted = i
            elif self.keys[i] == key:
                self.vals[i] = value  # update
                return
            i = (i + 1) % self.size
        
        slot = first_deleted if first_deleted != -1 else i
        self.keys[slot] = key
        self.vals[slot] = value
        self.count += 1
    
    def get(self, key: int) -> int:
        h = self._hash(key)
        i = h
        while self.keys[i] is not None:
            if self.keys[i] == key:
                return self.vals[i]
            i = (i + 1) % self.size
            if i == h:  # full circle
                break
        return -1
    
    def remove(self, key: int) -> None:
        h = self._hash(key)
        i = h
        while self.keys[i] is not None:
            if self.keys[i] == key:
                self.keys[i] = self._DELETED  # tombstone
                self.count -= 1
                return
            i = (i + 1) % self.size
```

---

## 4. Design HashSet

```python
class MyHashSet:
    """
    HashSet from scratch using bit arrays (memory efficient).
    
    For integer keys 0 to MAX:
    - Bit array of size MAX // 64 + 1
    - Each integer is one bit → 8x more memory efficient than boolean array
    
    Time: O(1) add, remove, contains
    Space: O(MAX / 8) bytes — for MAX=10^6, only 125KB!
    """
    
    def __init__(self, max_val: int = 10**6):
        self.bits = bytearray((max_val >> 3) + 1)  # each byte = 8 elements
    
    def add(self, key: int) -> None:
        self.bits[key >> 3] |= (1 << (key & 7))
    
    def remove(self, key: int) -> None:
        self.bits[key >> 3] &= ~(1 << (key & 7))
    
    def contains(self, key: int) -> bool:
        return bool(self.bits[key >> 3] & (1 << (key & 7)))


# Chaining-based HashSet for general keys:
class MyHashSetGeneral:
    """HashSet with chaining. Works for any hashable key."""
    
    def __init__(self, size: int = 1000):
        self.buckets = [[] for _ in range(size)]
        self.size = size
    
    def _hash(self, key) -> int:
        return hash(key) % self.size
    
    def add(self, key) -> None:
        h = self._hash(key)
        if key not in self.buckets[h]:
            self.buckets[h].append(key)
    
    def remove(self, key) -> None:
        h = self._hash(key)
        if key in self.buckets[h]:
            self.buckets[h].remove(key)
    
    def contains(self, key) -> bool:
        h = self._hash(key)
        return key in self.buckets[h]
```

---

## 5. Design Twitter — Top 10 Feed

```python
import heapq
from collections import defaultdict

class Twitter:
    """
    Design Twitter with:
    - postTweet(userId, tweetId): O(1)
    - getNewsFeed(userId): Returns 10 most recent tweets from user and followees. O(F log F + 10 log F) where F = # followees
    - follow(followerId, followeeId): O(1)
    - unfollow(followerId, followeeId): O(1)
    
    Key design:
    - Global timestamp for ordering
    - Each user has a list of (timestamp, tweetId) — latest first
    - getNewsFeed: merge K sorted lists using min-heap (K = # followees + self)
    
    Time: post O(1), follow/unfollow O(1), getNewsFeed O(F log F + 10 log F)
    Space: O(U + T) where U = users, T = tweets
    """
    
    def __init__(self):
        self.timestamp = 0
        self.tweets = defaultdict(list)   # userId → [(ts, tweetId)]
        self.following = defaultdict(set) # userId → set of followeeIds
    
    def postTweet(self, userId: int, tweetId: int) -> None:
        """O(1)"""
        self.tweets[userId].append((self.timestamp, tweetId))
        self.timestamp += 1
    
    def getNewsFeed(self, userId: int) -> list[int]:
        """
        Get 10 most recent tweets from user + followees.
        
        Algorithm: K-way merge using min-heap.
        - Start each person's pointer at their latest tweet
        - Use max-heap (negate timestamp for max behavior)
        - Extract 10 times
        
        Time: O(F log F + 10 log F) = O(F log F) where F = # followees
        """
        # All relevant users: self + followees
        users = self.following[userId] | {userId}
        
        # Min-heap: (-timestamp, tweetId, userId, tweet_index)
        heap = []
        for uid in users:
            user_tweets = self.tweets[uid]
            if user_tweets:
                idx = len(user_tweets) - 1  # start from latest
                ts, tid = user_tweets[idx]
                heapq.heappush(heap, (-ts, tid, uid, idx))
        
        result = []
        while heap and len(result) < 10:
            neg_ts, tid, uid, idx = heapq.heappop(heap)
            result.append(tid)
            if idx > 0:  # more tweets from this user
                nts, ntid = self.tweets[uid][idx - 1]
                heapq.heappush(heap, (-nts, ntid, uid, idx - 1))
        
        return result
    
    def follow(self, followerId: int, followeeId: int) -> None:
        """O(1)"""
        if followerId != followeeId:
            self.following[followerId].add(followeeId)
    
    def unfollow(self, followerId: int, followeeId: int) -> None:
        """O(1)"""
        self.following[followerId].discard(followeeId)


# ─── Test ───
t = Twitter()
t.postTweet(1, 5)
print(t.getNewsFeed(1))  # [5]
t.follow(1, 2)
t.postTweet(2, 6)
print(t.getNewsFeed(1))  # [6, 5] (6 is newer)
t.unfollow(1, 2)
print(t.getNewsFeed(1))  # [5] (no longer following 2)
```

---

## 6. Time-Based Key-Value Store

```python
from bisect import bisect_right
from collections import defaultdict

class TimeMap:
    """
    Time-based key-value store.
    - set(key, value, timestamp): Store (key, value) at timestamp. O(1)
    - get(key, timestamp): Return value with largest ts ≤ given ts. O(log N)
    
    Guarantee: timestamps for set() calls are strictly increasing per key.
    
    Implementation: For each key, maintain sorted list of (timestamp, value).
    Binary search to find the right value.
    
    Time: set O(1), get O(log N)
    Space: O(N)
    """
    
    def __init__(self):
        self.store = defaultdict(list)  # key → [(timestamp, value)]
    
    def set(self, key: str, value: str, timestamp: int) -> None:
        """Store key=value at timestamp. Timestamps are monotonically increasing."""
        self.store[key].append((timestamp, value))
    
    def get(self, key: str, timestamp: int) -> str:
        """
        Return value at latest timestamp <= given timestamp.
        Returns "" if no such timestamp exists.
        Time: O(log N) binary search
        """
        if key not in self.store:
            return ""
        
        entries = self.store[key]  # sorted by timestamp (guaranteed)
        
        # Binary search: find rightmost timestamp <= given timestamp
        # bisect_right finds insertion point after all equal timestamps
        idx = bisect_right(entries, (timestamp, chr(127)))  # chr(127) > any value
        # Actually: bisect_right(entries, (timestamp,)) with padding trick
        
        # Simpler: binary search manually
        lo, hi = 0, len(entries) - 1
        result = ""
        while lo <= hi:
            mid = (lo + hi) // 2
            if entries[mid][0] <= timestamp:
                result = entries[mid][1]
                lo = mid + 1
            else:
                hi = mid - 1
        return result


# ─── Test ───
tm = TimeMap()
tm.set("foo", "bar", 1)
print(tm.get("foo", 1))   # "bar"
print(tm.get("foo", 3))   # "bar" (most recent ≤ 3)
tm.set("foo", "bar2", 4)
print(tm.get("foo", 4))   # "bar2"
print(tm.get("foo", 5))   # "bar2"
print(tm.get("foo", 0))   # "" (no entry ≤ 0)
```

---

## 7. All-O(1) Data Structure

```python
class AllOne:
    """
    Data structure supporting:
    - inc(key): Increment frequency of key. O(1)
    - dec(key): Decrement frequency of key (remove if freq→0). O(1)
    - getMaxKey(): Return any key with max frequency. O(1)
    - getMinKey(): Return any key with min frequency. O(1)
    
    Key insight: Maintain a doubly linked list of "frequency buckets",
    each containing a set of keys with that frequency.
    List is ordered by frequency: low_sentinel ↔ [freq=1] ↔ [freq=2] ↔ ... ↔ high_sentinel
    
    Similar structure to LFU cache's freq_map!
    
    Time: All operations O(1)
    Space: O(N)
    """
    
    class Bucket:
        def __init__(self, freq: int):
            self.freq = freq
            self.keys = set()
            self.prev = self.next = None
    
    def __init__(self):
        self.key_freq = {}   # key → frequency
        self.freq_bucket = {}  # freq → Bucket node
        
        # Sentinel buckets for min/max tracking
        self.lo = self.Bucket(0)   # sentinel for minimum end
        self.hi = self.Bucket(0)   # sentinel for maximum end
        self.lo.next = self.hi
        self.hi.prev = self.lo
    
    def _insert_after(self, node, new_node):
        """Insert new_node after node."""
        new_node.prev = node
        new_node.next = node.next
        node.next.prev = new_node
        node.next = new_node
    
    def _remove_bucket(self, bucket):
        """Remove empty bucket from the list."""
        bucket.prev.next = bucket.next
        bucket.next.prev = bucket.prev
        del self.freq_bucket[bucket.freq]
    
    def inc(self, key: str) -> None:
        if key in self.key_freq:
            old_freq = self.key_freq[key]
            new_freq = old_freq + 1
            self.key_freq[key] = new_freq
            
            old_bucket = self.freq_bucket[old_freq]
            old_bucket.keys.discard(key)
            
            # Get or create bucket for new_freq
            if new_freq not in self.freq_bucket:
                new_bucket = self.Bucket(new_freq)
                self._insert_after(old_bucket, new_bucket)
                self.freq_bucket[new_freq] = new_bucket
            
            self.freq_bucket[new_freq].keys.add(key)
            
            if not old_bucket.keys:
                self._remove_bucket(old_bucket)
        else:
            # New key with freq = 1
            self.key_freq[key] = 1
            if 1 not in self.freq_bucket:
                bucket = self.Bucket(1)
                self._insert_after(self.lo, bucket)
                self.freq_bucket[1] = bucket
            self.freq_bucket[1].keys.add(key)
    
    def dec(self, key: str) -> None:
        if key not in self.key_freq:
            return
        
        old_freq = self.key_freq[key]
        new_freq = old_freq - 1
        
        old_bucket = self.freq_bucket[old_freq]
        old_bucket.keys.discard(key)
        
        if new_freq == 0:
            del self.key_freq[key]
        else:
            self.key_freq[key] = new_freq
            if new_freq not in self.freq_bucket:
                new_bucket = self.Bucket(new_freq)
                self._insert_after(old_bucket.prev, new_bucket)
                self.freq_bucket[new_freq] = new_bucket
            self.freq_bucket[new_freq].keys.add(key)
        
        if not old_bucket.keys:
            self._remove_bucket(old_bucket)
    
    def getMaxKey(self) -> str:
        if self.hi.prev == self.lo:
            return ""
        return next(iter(self.hi.prev.keys))
    
    def getMinKey(self) -> str:
        if self.lo.next == self.hi:
            return ""
        return next(iter(self.lo.next.keys))
```

---

## 8. Design Phone Directory

```python
class PhoneDirectory:
    """
    Phone directory: get available number, check availability, release number.
    - get(): Return available number. O(1)
    - check(number): Is number available? O(1)
    - release(number): Release number back to pool. O(1)
    
    Uses a set for available numbers (O(1) all operations).
    """
    
    def __init__(self, maxNumbers: int):
        self.available = set(range(maxNumbers))
        self.in_use = set()
    
    def get(self) -> int:
        """Return any available number. O(1) amortized."""
        if not self.available:
            return -1
        num = next(iter(self.available))
        self.available.remove(num)
        self.in_use.add(num)
        return num
    
    def check(self, number: int) -> bool:
        """Is number available? O(1)."""
        return number in self.available
    
    def release(self, number: int) -> None:
        """Release number. O(1)."""
        if number in self.in_use:
            self.in_use.remove(number)
            self.available.add(number)
```

---

## 9. Random Pick Index — Reservoir Sampling

```python
import random

class Solution:
    """
    Reservoir Sampling: pick random index of target in large/streaming array.
    
    Algorithm: When processing i-th occurrence of target,
    select it with probability 1/i (replace current pick with probability 1/i).
    
    Proof: P(i-th element chosen) = (1/i) * (i/(i+1)) * ((i+1)/(i+2)) * ... = 1/k
    where k is total count. Each element has equal probability 1/k. ✓
    
    Time: O(N) per pick (must scan all) | Space: O(1)
    
    Use case: When array is too large to preprocess all indices.
    """
    
    def __init__(self, nums: list[int]):
        self.nums = nums
    
    def pick(self, target: int) -> int:
        """Return random index where nums[index] == target. O(N)."""
        result = -1
        count = 0
        
        for i, x in enumerate(self.nums):
            if x == target:
                count += 1
                # Replace result with probability 1/count
                if random.random() < 1.0 / count:
                    result = i
        
        return result


# Preprocessed version (when space allows):
class SolutionFast:
    """
    Preprocessed: store all indices per value.
    O(N) build, O(1) pick.
    Space: O(N)
    """
    from collections import defaultdict
    
    def __init__(self, nums: list[int]):
        self.indices = defaultdict(list)
        for i, x in enumerate(nums):
            self.indices[x].append(i)
    
    def pick(self, target: int) -> int:
        return random.choice(self.indices[target])
```

---

## 10. Design Rate Limiter

```python
import time
from collections import deque

class TokenBucketRateLimiter:
    """
    Token Bucket Rate Limiter.
    - capacity: max tokens in bucket
    - refill_rate: tokens added per second
    - Burst traffic allowed up to capacity.
    
    Time: O(1) per request
    Space: O(1) per user
    """
    
    def __init__(self, capacity: int, refill_rate: float):
        self.capacity = capacity
        self.refill_rate = refill_rate
        self.tokens = capacity
        self.last_refill = time.time()
    
    def allow_request(self) -> bool:
        """Check if request is allowed. O(1)."""
        now = time.time()
        elapsed = now - self.last_refill
        
        # Add tokens for elapsed time
        self.tokens = min(
            self.capacity,
            self.tokens + elapsed * self.refill_rate
        )
        self.last_refill = now
        
        if self.tokens >= 1:
            self.tokens -= 1
            return True
        return False


class SlidingWindowRateLimiter:
    """
    Sliding Window Log Rate Limiter.
    - Allows at most `max_requests` in any rolling `window_seconds` window.
    
    Time: O(1) amortized (cleanup old requests)
    Space: O(max_requests)
    """
    
    def __init__(self, max_requests: int, window_seconds: int):
        self.max_requests = max_requests
        self.window_seconds = window_seconds
        self.requests = deque()  # timestamps of recent requests
    
    def allow_request(self, user_id: int = None) -> bool:
        """Check if request is within rate limit. O(1) amortized."""
        now = time.time()
        window_start = now - self.window_seconds
        
        # Remove expired requests
        while self.requests and self.requests[0] < window_start:
            self.requests.popleft()
        
        if len(self.requests) < self.max_requests:
            self.requests.append(now)
            return True
        return False


class FixedWindowRateLimiter:
    """
    Fixed Window Counter Rate Limiter.
    Simple but has burst issue at window boundaries.
    
    Time: O(1) | Space: O(1)
    """
    
    def __init__(self, max_requests: int, window_seconds: int):
        self.max_requests = max_requests
        self.window_seconds = window_seconds
        self.count = 0
        self.window_start = time.time()
    
    def allow_request(self) -> bool:
        now = time.time()
        
        if now - self.window_start >= self.window_seconds:
            # New window
            self.count = 0
            self.window_start = now
        
        if self.count < self.max_requests:
            self.count += 1
            return True
        return False
```

---

## 11. Interview Tips & Edge Cases

### ⚡ LRU Common Mistakes

```python
# MISTAKE 1: Forgetting to update on get()
# Wrong: def get(self, key): return self.cache.get(key, -1)
# Right: must move node to front on access

# MISTAKE 2: Evicting BEFORE checking if key already exists
# Wrong: evict first, then check
# Right: check cache first; if key exists, just update

# MISTAKE 3: Not handling capacity=0
def put_safe(self, key, value):
    if self.capacity == 0:  # Edge case!
        return
    # ... rest of put

# MISTAKE 4: Memory leak — not deleting from cache dict when evicting
# Wrong: just remove from linked list
# Right: del self.cache[lru.key] AND remove from list
```

### ⚡ LFU Common Mistakes

```python
# MISTAKE 1: Not updating min_freq on increment
# When freq_map[min_freq] becomes empty after increment, min_freq += 1

# MISTAKE 2: Forgetting min_freq = 1 on new insert
# New keys always have freq=1, so min_freq = 1 after any put with new key

# MISTAKE 3: put() called with key that already exists
# Must UPDATE (not insert) and increment frequency

# MISTAKE 4: capacity=0 edge case
if self.capacity <= 0:
    return  # immediately return
```

### 🔑 Complexity Summary

| Structure         | Get        | Put        | Space    |
| -------------------| ------------| ------------| ----------|
| LRU Cache         | O(1)       | O(1)       | O(N)     |
| LFU Cache         | O(1)       | O(1)       | O(N)     |
| HashMap (avg)     | O(1)       | O(1)       | O(N)     |
| HashMap (worst)   | O(N)       | O(N)       | O(N)     |
| HashSet (bits)    | O(1)       | O(1)       | O(MAX/8) |
| Twitter feed      | O(1)       | O(F log F) | O(U+T)   |
| TimeMap           | O(1)       | O(log N)   | O(N)     |
| AllOne            | O(1)       | O(1)       | O(N)     |
| Token Bucket      | O(1)       | —          | O(1)     |
| Sliding Window RL | O(1) amort | —          | O(W)     |

### 📋 Interview Script for LRU

> "I'll use a doubly linked list for recency order and a hashmap for O(1) access. Sentinel head and tail nodes eliminate null checks. When we get a key, we move it to the front. When we put: if existing, update and move to front; if new and at capacity, evict from tail, then add to front. This gives O(1) for both operations."

### 🏆 Follow-Up Questions to Expect

1. **"How would you make this thread-safe?"** → Add `threading.Lock()`, use `with self.lock:` in get/put
2. **"How would you persist this to disk?"** → Write-ahead log (WAL) + periodic snapshots
3. **"How would you handle distributed LRU?"** → Consistent hashing + Redis with LRU eviction policy
4. **"What if capacity is very large (1 billion)?"** → Sharding across multiple nodes
5. **"LRU vs LFU — when to choose?"** → LRU: temporal locality (recent = likely needed). LFU: frequency locality (popular items). LFU handles "cache pollution" better but has higher implementation complexity.

---

*The LRU/LFU cache is the #1 most commonly tested design problem across all FAANG companies. Practice implementing LRU from scratch (without OrderedDict) in 15 minutes. LFU in 25 minutes. Both with full edge case handling.*
