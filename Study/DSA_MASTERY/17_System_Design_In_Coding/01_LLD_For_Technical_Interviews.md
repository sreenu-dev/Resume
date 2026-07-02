# Low-Level Design (LLD) for Technical Interviews — Advanced Mastery Guide

> **Level:** Advanced | **Prerequisites:** OOP, Design Patterns, Data Structures  
> **Interview Frequency:** Google ★★★☆☆ | Meta ★★★☆☆ | Amazon ★★★★★ | Microsoft ★★★★★

---

## Table of Contents
1. [LLD Interview Format & Evaluation](#1-lld-interview-format--evaluation)
2. [SOLID Principles in Practice](#2-solid-principles-in-practice)
3. [Design: Parking Lot System](#3-design-parking-lot-system)
4. [Design: Vending Machine](#4-design-vending-machine)
5. [Design: Snake and Ladder Game](#5-design-snake-and-ladder-game)
6. [Design: Elevator System](#6-design-elevator-system)
7. [Design: BookMyShow (Ticket Booking)](#7-design-bookmyshow-ticket-booking)
8. [Design: Splitwise (Expense Sharing)](#8-design-splitwise-expense-sharing)
9. [Design: Rate Limiter — Code-Level](#9-design-rate-limiter--code-level)
10. [Interview Tips & Communication Script](#10-interview-tips--communication-script)

---

## 1. LLD Interview Format & Evaluation

### What Interviewers Evaluate

| Dimension | What They Look For | Red Flags |
|-----------|-------------------|-----------|
| **Class design** | Correct abstraction, single responsibility | God classes, everything in one class |
| **Extensibility** | Open/closed principle | Hard-coded conditionals everywhere |
| **Data modeling** | Right data structures for requirements | Using List when HashMap is needed |
| **Concurrency** | Thread safety awareness | Never mentioning race conditions |
| **API design** | Clean method signatures | Too many params, unclear names |
| **Edge cases** | Handling null, full capacity, invalid input | "I'll handle that later" |

### LLD Interview Flow (45 minutes)
```
Minutes 0-5:   Clarify requirements — ask questions!
Minutes 5-15:  Define entities (classes) and their relationships
Minutes 15-30: Implement core classes with methods
Minutes 30-40: Handle edge cases and extend
Minutes 40-45: Discuss scalability and trade-offs
```

### Questions to ALWAYS Ask
- "What are the main actors/users?"
- "What operations need to be supported?"
- "What are the capacity constraints?"
- "Do we need concurrency support?"
- "Any persistence requirements?"

---

## 2. SOLID Principles in Practice

```python
# ─── S: Single Responsibility ───
# BAD: One class doing everything
class BadVehicle:
    def park(self): ...
    def calculate_fee(self): ...
    def send_notification(self): ...
    def update_database(self): ...

# GOOD: Each class has one reason to change
class Vehicle: pass                    # Just vehicle data
class ParkingFeeCalculator: pass       # Just fee calculation
class NotificationService: pass        # Just notifications

# ─── O: Open/Closed ───
# BAD: Adding new vehicle type requires modifying existing code
class BadCalculator:
    def calculate(self, vehicle_type):
        if vehicle_type == "car": return 20
        elif vehicle_type == "bike": return 10
        # Need to modify this method for every new type!

# GOOD: Extend without modifying
from abc import ABC, abstractmethod

class FeeStrategy(ABC):
    @abstractmethod
    def calculate(self, hours: float) -> float: ...

class CarFeeStrategy(FeeStrategy):
    def calculate(self, hours: float) -> float:
        return hours * 20

class BikeFeeStrategy(FeeStrategy):
    def calculate(self, hours: float) -> float:
        return hours * 10

class ElectricCarFeeStrategy(FeeStrategy):  # New type: no existing code changed!
    def calculate(self, hours: float) -> float:
        return hours * 15

# ─── L: Liskov Substitution ───
# Subtypes must be substitutable for their base types
class Shape(ABC):
    @abstractmethod
    def area(self) -> float: ...

class Rectangle(Shape):
    def __init__(self, w, h): self.w, self.h = w, h
    def area(self): return self.w * self.h

class Square(Shape):  # Correct: Square IS-A Shape (not Rectangle)
    def __init__(self, s): self.s = s
    def area(self): return self.s * self.s

# ─── I: Interface Segregation ───
# Don't force classes to implement interfaces they don't use
class Reservable(ABC):
    @abstractmethod
    def reserve(self): ...

class Parkable(ABC):
    @abstractmethod
    def park(self): ...

# Motorcycle implements only what it needs
class Motorcycle(Parkable):
    def park(self): pass
    # No need to implement reserve() if motorcycles can't be reserved

# ─── D: Dependency Inversion ───
# High-level modules should not depend on low-level modules
class ParkingLot:
    def __init__(self, payment_service: 'PaymentService'):
        # Depend on abstraction, not concrete implementation
        self.payment = payment_service
    
    def checkout(self, ticket):
        fee = self.calculate_fee(ticket)
        self.payment.charge(fee)  # Works with any PaymentService

class StripePayment:  # Concrete implementation
    def charge(self, amount): ...

class PayPalPayment:  # Alternative — inject at runtime
    def charge(self, amount): ...
```

---

## 3. Design: Parking Lot System

```python
from enum import Enum
from datetime import datetime
from abc import ABC, abstractmethod
from typing import Optional
import threading

class VehicleType(Enum):
    MOTORCYCLE = "motorcycle"
    CAR = "car"
    TRUCK = "truck"
    ELECTRIC = "electric"

class Vehicle:
    def __init__(self, license_plate: str, vehicle_type: VehicleType):
        self.license_plate = license_plate
        self.vehicle_type = vehicle_type
    
    def __repr__(self):
        return f"Vehicle({self.license_plate}, {self.vehicle_type.value})"

class SpotType(Enum):
    COMPACT = "compact"        # motorcycles only
    REGULAR = "regular"        # cars and motorcycles
    LARGE = "large"            # trucks, cars, motorcycles
    ELECTRIC = "electric"      # electric vehicles (has charger)

class ParkingSpot:
    def __init__(self, spot_id: str, spot_type: SpotType, floor: int):
        self.spot_id = spot_id
        self.spot_type = spot_type
        self.floor = floor
        self.vehicle: Optional[Vehicle] = None
        self._lock = threading.Lock()
    
    def is_available(self) -> bool:
        return self.vehicle is None
    
    def can_fit(self, vehicle: Vehicle) -> bool:
        """Check if this spot can accommodate the vehicle."""
        if self.spot_type == SpotType.COMPACT:
            return vehicle.vehicle_type == VehicleType.MOTORCYCLE
        elif self.spot_type == SpotType.REGULAR:
            return vehicle.vehicle_type in [VehicleType.CAR, VehicleType.MOTORCYCLE]
        elif self.spot_type == SpotType.LARGE:
            return True  # Any vehicle
        elif self.spot_type == SpotType.ELECTRIC:
            return vehicle.vehicle_type == VehicleType.ELECTRIC
        return False
    
    def park(self, vehicle: Vehicle) -> bool:
        """Thread-safe parking. Returns True if successful."""
        with self._lock:
            if self.is_available() and self.can_fit(vehicle):
                self.vehicle = vehicle
                return True
            return False
    
    def unpark(self) -> Optional[Vehicle]:
        """Remove vehicle from spot. Thread-safe."""
        with self._lock:
            vehicle = self.vehicle
            self.vehicle = None
            return vehicle

class ParkingTicket:
    _counter = 0
    _lock = threading.Lock()
    
    def __init__(self, vehicle: Vehicle, spot: ParkingSpot):
        with ParkingTicket._lock:
            ParkingTicket._counter += 1
            self.ticket_id = ParkingTicket._counter
        self.vehicle = vehicle
        self.spot = spot
        self.entry_time = datetime.now()
        self.exit_time: Optional[datetime] = None
        self.fee: float = 0.0
    
    def duration_hours(self) -> float:
        end = self.exit_time or datetime.now()
        return (end - self.entry_time).total_seconds() / 3600

class FeeStrategy(ABC):
    @abstractmethod
    def calculate(self, hours: float) -> float: ...

class HourlyFeeStrategy(FeeStrategy):
    RATES = {
        VehicleType.MOTORCYCLE: 5.0,
        VehicleType.CAR: 20.0,
        VehicleType.TRUCK: 40.0,
        VehicleType.ELECTRIC: 15.0,
    }
    
    def calculate(self, hours: float, vehicle_type: VehicleType = VehicleType.CAR) -> float:
        rate = self.RATES.get(vehicle_type, 20.0)
        return max(1.0, hours) * rate  # minimum 1 hour charge

class ParkingFloor:
    def __init__(self, floor_num: int, spots: list[ParkingSpot]):
        self.floor_num = floor_num
        self.spots = spots
        self._available = {s.spot_id: s for s in spots if s.is_available()}
    
    def find_spot(self, vehicle: Vehicle) -> Optional[ParkingSpot]:
        """Find first available spot for vehicle. O(N) worst case."""
        for spot in self.spots:
            if spot.is_available() and spot.can_fit(vehicle):
                return spot
        return None
    
    def available_count(self) -> int:
        return sum(1 for s in self.spots if s.is_available())

class ParkingLot:
    """
    Main parking lot class.
    
    Design decisions:
    - Each floor manages its spots
    - Tickets are the primary tracking mechanism
    - Thread-safe spot allocation
    - Strategy pattern for fee calculation (extensible)
    
    Time: park O(F*S), exit O(1), find_ticket O(1)
    Space: O(F*S + T) where F=floors, S=spots/floor, T=active tickets
    """
    
    def __init__(self, name: str, floors: list[ParkingFloor], 
                 fee_strategy: FeeStrategy = None):
        self.name = name
        self.floors = floors
        self.fee_strategy = fee_strategy or HourlyFeeStrategy()
        self.active_tickets: dict[int, ParkingTicket] = {}
        self._lock = threading.Lock()
    
    def park(self, vehicle: Vehicle) -> Optional[ParkingTicket]:
        """
        Park vehicle. Returns ticket or None if full.
        Thread-safe: uses floor-level spot locking.
        """
        for floor in self.floors:
            spot = floor.find_spot(vehicle)
            if spot and spot.park(vehicle):
                ticket = ParkingTicket(vehicle, spot)
                with self._lock:
                    self.active_tickets[ticket.ticket_id] = ticket
                print(f"Parked {vehicle} at spot {spot.spot_id}, ticket #{ticket.ticket_id}")
                return ticket
        
        print(f"Parking full! Cannot park {vehicle}")
        return None
    
    def exit(self, ticket_id: int) -> float:
        """
        Process exit. Returns fee charged.
        Validates ticket, calculates fee, frees spot.
        """
        with self._lock:
            ticket = self.active_tickets.pop(ticket_id, None)
        
        if not ticket:
            raise ValueError(f"Invalid ticket: {ticket_id}")
        
        ticket.exit_time = datetime.now()
        hours = ticket.duration_hours()
        ticket.fee = self.fee_strategy.calculate(hours)
        ticket.spot.unpark()
        
        print(f"Exit: ticket #{ticket_id}, {hours:.2f}hrs, fee=${ticket.fee:.2f}")
        return ticket.fee
    
    def available_spots(self) -> dict:
        """Overview of available spots per floor."""
        return {f.floor_num: f.available_count() for f in self.floors}
    
    def is_full(self) -> bool:
        return all(f.available_count() == 0 for f in self.floors)


# ─── Factory for building parking lot ───
class ParkingLotFactory:
    @staticmethod
    def create_standard_lot(floors: int, spots_per_floor: int) -> ParkingLot:
        parking_floors = []
        for f in range(floors):
            spots = []
            for s in range(spots_per_floor):
                spot_id = f"F{f}S{s}"
                if s < spots_per_floor // 10:
                    spot_type = SpotType.COMPACT
                elif s < spots_per_floor // 2:
                    spot_type = SpotType.REGULAR
                else:
                    spot_type = SpotType.LARGE
                spots.append(ParkingSpot(spot_id, spot_type, f))
            parking_floors.append(ParkingFloor(f, spots))
        return ParkingLot("Standard Lot", parking_floors)


# ─── Test ───
lot = ParkingLotFactory.create_standard_lot(floors=3, spots_per_floor=10)
car = Vehicle("ABC123", VehicleType.CAR)
bike = Vehicle("BIKE01", VehicleType.MOTORCYCLE)

ticket1 = lot.park(car)
ticket2 = lot.park(bike)
print(lot.available_spots())
import time; time.sleep(0.01)  # Simulate time passing
lot.exit(ticket1.ticket_id)
```

---

## 4. Design: Vending Machine

```python
class VendingMachineState(Enum):
    IDLE = "idle"
    MONEY_INSERTED = "money_inserted"
    DISPENSING = "dispensing"
    MAINTENANCE = "maintenance"

class Item:
    def __init__(self, name: str, price: float, quantity: int):
        self.name = name
        self.price = price
        self.quantity = quantity

class VendingMachine:
    """
    Vending Machine using State Pattern.
    States: IDLE → MONEY_INSERTED → DISPENSING → IDLE
    
    Operations:
    - insert_money(amount): Add money
    - select_item(item_code): Select product
    - dispense(): Get product + change
    - cancel(): Get refund
    """
    
    def __init__(self):
        self.inventory: dict[str, Item] = {}
        self.inserted_amount: float = 0.0
        self.selected_item: Optional[str] = None
        self.state = VendingMachineState.IDLE
    
    def add_item(self, code: str, item: Item):
        self.inventory[code] = item
    
    def insert_money(self, amount: float) -> str:
        if self.state == VendingMachineState.MAINTENANCE:
            return "Machine under maintenance"
        if amount <= 0:
            return "Invalid amount"
        self.inserted_amount += amount
        self.state = VendingMachineState.MONEY_INSERTED
        return f"Inserted ${amount:.2f}. Total: ${self.inserted_amount:.2f}"
    
    def select_item(self, code: str) -> str:
        if self.state == VendingMachineState.IDLE:
            return "Please insert money first"
        if code not in self.inventory:
            return "Item not found"
        item = self.inventory[code]
        if item.quantity == 0:
            return f"{item.name} is out of stock"
        if self.inserted_amount < item.price:
            return f"Insufficient funds. Need ${item.price - self.inserted_amount:.2f} more"
        self.selected_item = code
        return f"Selected: {item.name} (${item.price:.2f})"
    
    def dispense(self) -> tuple[str, float]:
        if not self.selected_item:
            return "No item selected", 0.0
        if self.state != VendingMachineState.MONEY_INSERTED:
            return "Please insert money and select item", 0.0
        
        self.state = VendingMachineState.DISPENSING
        item = self.inventory[self.selected_item]
        change = self.inserted_amount - item.price
        item.quantity -= 1
        
        # Reset state
        self.inserted_amount = 0.0
        self.selected_item = None
        self.state = VendingMachineState.IDLE
        
        return f"Dispensed: {item.name}", change
    
    def cancel(self) -> float:
        refund = self.inserted_amount
        self.inserted_amount = 0.0
        self.selected_item = None
        self.state = VendingMachineState.IDLE
        return refund
```

---

## 5. Design: Snake and Ladder Game

```python
import random
from collections import deque

class Player:
    def __init__(self, name: str, token_color: str = "red"):
        self.name = name
        self.position = 0  # Start before board
        self.token_color = token_color
    
    def __repr__(self):
        return f"Player({self.name}, pos={self.position})"

class Dice:
    def __init__(self, sides: int = 6, count: int = 1):
        self.sides = sides
        self.count = count
    
    def roll(self) -> int:
        return sum(random.randint(1, self.sides) for _ in range(self.count))

class Board:
    def __init__(self, size: int = 100):
        self.size = size
        self.jumps: dict[int, int] = {}  # position → destination (snake or ladder)
    
    def add_ladder(self, start: int, end: int):
        assert start < end, "Ladder must go up"
        assert 1 <= start <= self.size and 1 <= end <= self.size
        self.jumps[start] = end
    
    def add_snake(self, head: int, tail: int):
        assert head > tail, "Snake must go down"
        assert 1 <= head <= self.size and 1 <= tail <= self.size
        self.jumps[head] = tail
    
    def apply_jump(self, position: int) -> int:
        return self.jumps.get(position, position)

class SnakeLadderGame:
    """
    Snake and Ladder game.
    
    Also features:
    - BFS to find minimum dice rolls to win (classic interview extension!)
    
    Design patterns used: Strategy (Dice), Template Method (game loop)
    """
    
    def __init__(self, players: list[Player], board: Board, dice: Dice):
        self.players = deque(players)  # rotate for turns
        self.board = board
        self.dice = dice
        self.winner = None
        self.turn_count = 0
    
    def play_turn(self) -> str:
        if self.winner:
            return f"Game over! {self.winner.name} won!"
        
        player = self.players[0]
        roll = self.dice.roll()
        new_pos = player.position + roll
        
        log = f"{player.name} rolled {roll}. {player.position} → "
        
        if new_pos > self.board.size:
            log += f"{player.position} (cannot move, need exact roll)"
        else:
            new_pos = self.board.apply_jump(new_pos)
            player.position = new_pos
            log += f"{new_pos}"
            
            if player.position == self.board.size:
                self.winner = player
                log += " 🏆 WINS!"
            elif new_pos in self.board.jumps:
                dest = self.board.jumps[new_pos]
                direction = "🐍 Snake" if dest < new_pos else "🪜 Ladder"
                log += f" {direction}!"
        
        self.players.rotate(-1)  # Next player's turn
        self.turn_count += 1
        return log
    
    def play_full_game(self, max_turns: int = 1000) -> str:
        turns = 0
        while not self.winner and turns < max_turns:
            print(self.play_turn())
            turns += 1
        return f"Winner: {self.winner.name if self.winner else 'No winner'}"
    
    @staticmethod
    def min_moves_to_win(board: Board) -> int:
        """
        BFS to find minimum dice rolls to reach board.size from 0.
        Classic interview extension!
        
        Time: O(board.size * dice_sides) | Space: O(board.size)
        """
        from collections import deque
        
        visited = set()
        queue = deque([(0, 0)])  # (position, moves)
        visited.add(0)
        
        while queue:
            pos, moves = queue.popleft()
            
            for roll in range(1, 7):
                next_pos = pos + roll
                if next_pos > board.size:
                    continue
                next_pos = board.apply_jump(next_pos)
                
                if next_pos == board.size:
                    return moves + 1
                
                if next_pos not in visited:
                    visited.add(next_pos)
                    queue.append((next_pos, moves + 1))
        
        return -1  # Unreachable


# ─── Setup ───
board = Board(100)
board.add_ladder(4, 14)
board.add_ladder(9, 31)
board.add_ladder(20, 38)
board.add_snake(17, 7)
board.add_snake(54, 34)
board.add_snake(62, 19)

print("Min moves:", SnakeLadderGame.min_moves_to_win(board))
```

---

## 6. Design: Elevator System

```python
from enum import Enum
import heapq

class Direction(Enum):
    UP = 1
    DOWN = -1
    IDLE = 0

class ElevatorRequest:
    def __init__(self, floor: int, direction: Direction):
        self.floor = floor
        self.direction = direction

class Elevator:
    """
    Single elevator with LOOK algorithm scheduling.
    LOOK: Service requests in current direction, then reverse.
    Better than FCFS for reducing total travel distance.
    """
    
    def __init__(self, elevator_id: int, current_floor: int = 0, 
                 min_floor: int = 0, max_floor: int = 20):
        self.id = elevator_id
        self.current_floor = current_floor
        self.direction = Direction.IDLE
        self.min_floor = min_floor
        self.max_floor = max_floor
        
        # LOOK algorithm: two heaps for pending floors
        self.up_requests = []    # min-heap: floors above current
        self.down_requests = []  # max-heap (negated): floors below current
        self.capacity = 10
        self.passengers = 0
    
    def request_floor(self, floor: int):
        """Add a floor to service. O(log N)."""
        if floor > self.current_floor:
            heapq.heappush(self.up_requests, floor)
        elif floor < self.current_floor:
            heapq.heappush(self.down_requests, -floor)
        # If same floor, door opens
    
    def next_floor(self) -> Optional[int]:
        """
        LOOK algorithm: get next floor to visit.
        Service in current direction first, then switch.
        """
        if self.direction == Direction.UP or self.direction == Direction.IDLE:
            if self.up_requests:
                self.direction = Direction.UP
                return heapq.heappop(self.up_requests)
            elif self.down_requests:
                self.direction = Direction.DOWN
                return -heapq.heappop(self.down_requests)
        else:  # Direction.DOWN
            if self.down_requests:
                return -heapq.heappop(self.down_requests)
            elif self.up_requests:
                self.direction = Direction.UP
                return heapq.heappop(self.up_requests)
        
        self.direction = Direction.IDLE
        return None
    
    def move_to(self, floor: int):
        print(f"Elevator {self.id}: {self.current_floor} → {floor}")
        self.current_floor = floor
    
    def is_idle(self) -> bool:
        return self.direction == Direction.IDLE and not self.up_requests and not self.down_requests
    
    def cost_to_service(self, floor: int) -> int:
        """Estimate cost (steps) to service this floor. For dispatcher."""
        return abs(self.current_floor - floor)

class ElevatorController:
    """
    Multi-elevator dispatcher.
    Strategy: assign request to elevator with minimum cost.
    
    Advanced: SCAN algorithm, zone-based assignment.
    """
    
    def __init__(self, num_elevators: int, floors: int):
        self.elevators = [
            Elevator(i, current_floor=0, max_floor=floors)
            for i in range(num_elevators)
        ]
        self.floors = floors
    
    def request(self, floor: int, direction: Direction):
        """
        Assign floor request to best elevator.
        Greedy: pick elevator with minimum travel cost.
        """
        best_elevator = min(
            self.elevators,
            key=lambda e: e.cost_to_service(floor)
        )
        best_elevator.request_floor(floor)
        print(f"Request: floor {floor} ({direction.name}) → Elevator {best_elevator.id}")
    
    def step(self):
        """Move each elevator one step."""
        for elevator in self.elevators:
            next_f = elevator.next_floor()
            if next_f is not None:
                elevator.move_to(next_f)
```

---

## 7. Design: BookMyShow (Ticket Booking)

```python
import threading
from datetime import datetime

class Seat:
    def __init__(self, seat_id: str, row: str, number: int, price: float):
        self.seat_id = seat_id
        self.row = row
        self.number = number
        self.price = price
        self.is_booked = False
        self._lock = threading.Lock()
    
    def book(self) -> bool:
        """Atomic booking with lock. Returns True if successfully booked."""
        with self._lock:
            if not self.is_booked:
                self.is_booked = True
                return True
            return False
    
    def cancel(self):
        with self._lock:
            self.is_booked = False

class Show:
    def __init__(self, show_id: str, movie: str, theatre: str, 
                 start_time: datetime, seats: list[Seat]):
        self.show_id = show_id
        self.movie = movie
        self.theatre = theatre
        self.start_time = start_time
        self.seats = {s.seat_id: s for s in seats}
        self._lock = threading.Lock()
    
    def available_seats(self) -> list[Seat]:
        return [s for s in self.seats.values() if not s.is_booked]
    
    def book_seats(self, seat_ids: list[str]) -> bool:
        """
        Book multiple seats atomically.
        All-or-nothing: if any seat unavailable, book none.
        Uses two-phase locking for atomicity.
        """
        seats = [self.seats[sid] for sid in seat_ids if sid in self.seats]
        if len(seats) != len(seat_ids):
            return False  # Some seats don't exist
        
        booked = []
        for seat in seats:
            if seat.book():
                booked.append(seat)
            else:
                # Rollback: cancel already-booked seats
                for s in booked:
                    s.cancel()
                return False
        
        return True

class Booking:
    _counter = 0
    
    def __init__(self, user_id: str, show: Show, seat_ids: list[str], total_price: float):
        Booking._counter += 1
        self.booking_id = f"BK{Booking._counter:06d}"
        self.user_id = user_id
        self.show = show
        self.seat_ids = seat_ids
        self.total_price = total_price
        self.status = "CONFIRMED"
        self.booking_time = datetime.now()

class BookingService:
    """
    Core booking service.
    
    Critical section: seat booking must be atomic.
    Real systems use DB transactions + optimistic locking.
    """
    
    def __init__(self):
        self.bookings: dict[str, Booking] = {}
        self.shows: dict[str, Show] = {}
    
    def add_show(self, show: Show):
        self.shows[show.show_id] = show
    
    def book(self, user_id: str, show_id: str, seat_ids: list[str]) -> Optional[Booking]:
        if show_id not in self.shows:
            raise ValueError("Show not found")
        
        show = self.shows[show_id]
        
        if not show.book_seats(seat_ids):
            print("Booking failed: seats unavailable")
            return None
        
        total = sum(show.seats[sid].price for sid in seat_ids)
        booking = Booking(user_id, show, seat_ids, total)
        self.bookings[booking.booking_id] = booking
        print(f"Booked: {booking.booking_id} for {user_id}, seats {seat_ids}, total ${total}")
        return booking
    
    def cancel(self, booking_id: str) -> bool:
        booking = self.bookings.get(booking_id)
        if not booking:
            return False
        for seat_id in booking.seat_ids:
            booking.show.seats[seat_id].cancel()
        booking.status = "CANCELLED"
        print(f"Cancelled: {booking_id}")
        return True
```

---

## 8. Design: Splitwise (Expense Sharing)

```python
from collections import defaultdict

class Expense:
    def __init__(self, amount: float, paid_by: str, 
                 split_among: list[str], split_type: str = "equal"):
        self.amount = amount
        self.paid_by = paid_by
        self.split_among = split_among
        self.split_type = split_type
        
        # Calculate each person's share
        if split_type == "equal":
            per_person = amount / len(split_among)
            self.shares = {person: per_person for person in split_among}
        # Could extend with "exact", "percentage" split types

class Splitwise:
    """
    Expense sharing with debt simplification.
    
    Core algorithm: net balance per person → greedy debt settlement
    Debt simplification reduces N transactions to at most N-1.
    
    Time: add_expense O(N), settle O(N log N)
    Space: O(N + E) where N = users, E = expenses
    """
    
    def __init__(self):
        self.users: set[str] = set()
        self.expenses: list[Expense] = []
        # balances[a][b] = amount a owes b (negative = b owes a)
        self.balances: dict[str, dict[str, float]] = defaultdict(lambda: defaultdict(float))
    
    def add_user(self, user: str):
        self.users.add(user)
    
    def add_expense(self, expense: Expense):
        """Add expense and update balances."""
        self.expenses.append(expense)
        self.users.add(expense.paid_by)
        
        for person, share in expense.shares.items():
            self.users.add(person)
            if person != expense.paid_by:
                # person owes paid_by
                self.balances[person][expense.paid_by] += share
    
    def net_balances(self) -> dict[str, float]:
        """
        Net balance for each user.
        Positive = others owe you
        Negative = you owe others
        """
        net = defaultdict(float)
        for debtor, creditors in self.balances.items():
            for creditor, amount in creditors.items():
                net[debtor] -= amount
                net[creditor] += amount
        return dict(net)
    
    def simplify_debts(self) -> list[tuple[str, str, float]]:
        """
        Simplify all debts to minimum transactions.
        Algorithm: Two-pointer on sorted net balances.
        
        Key insight: If person A has net -$30 and person B has net +$20,
        A pays B $20, A now has net -$10.
        
        This reduces N(N-1)/2 potential transactions to at most N-1.
        
        Time: O(N log N) | Space: O(N)
        """
        net = self.net_balances()
        
        # Separate into givers (negative net) and receivers (positive net)
        givers = sorted([(v, k) for k, v in net.items() if v < -0.001])
        receivers = sorted([(v, k) for k, v in net.items() if v > 0.001], reverse=True)
        
        transactions = []
        i, j = 0, 0
        
        while i < len(givers) and j < len(receivers):
            give_amt, giver = givers[i]  # give_amt is negative
            recv_amt, receiver = receivers[j]  # recv_amt is positive
            
            settle = min(-give_amt, recv_amt)
            transactions.append((giver, receiver, round(settle, 2)))
            
            give_amt += settle
            recv_amt -= settle
            
            if abs(give_amt) < 0.001:
                i += 1
            else:
                givers[i] = (give_amt, giver)
            
            if abs(recv_amt) < 0.001:
                j += 1
            else:
                receivers[j] = (recv_amt, receiver)
        
        return transactions


# ─── Test ───
sw = Splitwise()
sw.add_expense(Expense(90, "Alice", ["Alice", "Bob", "Charlie"]))  # 30 each
sw.add_expense(Expense(60, "Bob", ["Alice", "Bob"]))               # 30 each

transactions = sw.simplify_debts()
print("Settle debts:")
for giver, receiver, amount in transactions:
    print(f"  {giver} pays {receiver} ${amount:.2f}")
# Expected: Charlie pays Alice $30, Alice pays Bob $0... (net: Alice-0, Bob+0, Charlie-30)
```

---

## 9. Design: Rate Limiter — Code-Level

*(See File 04 for full token bucket and sliding window implementations.)*

```python
from abc import ABC, abstractmethod
from collections import defaultdict
import time

class RateLimitStrategy(ABC):
    @abstractmethod
    def is_allowed(self, user_id: str) -> bool: ...

class PerUserSlidingWindowLimiter(RateLimitStrategy):
    """Per-user rate limiting with sliding window."""
    
    def __init__(self, max_requests: int, window_secs: int):
        self.max_requests = max_requests
        self.window_secs = window_secs
        self.user_windows: dict[str, deque] = defaultdict(deque)
    
    def is_allowed(self, user_id: str) -> bool:
        now = time.time()
        window = self.user_windows[user_id]
        
        # Remove expired
        while window and window[0] < now - self.window_secs:
            window.popleft()
        
        if len(window) < self.max_requests:
            window.append(now)
            return True
        return False

class RateLimiterMiddleware:
    """Middleware applying rate limiting with fallback strategy."""
    
    def __init__(self, strategy: RateLimitStrategy):
        self.strategy = strategy
    
    def handle_request(self, user_id: str, request: dict) -> dict:
        if not self.strategy.is_allowed(user_id):
            return {"status": 429, "message": "Too Many Requests", 
                    "retry_after": "1 second"}
        return {"status": 200, "data": "processed"}
```

---

## 10. Interview Tips & Communication Script

### 📝 LLD Interview Script Template

```
Step 1 — Clarify (2 min):
"Before I start, let me clarify requirements:
 - Who are the main actors? [list them]
 - What are the key operations? [list them]
 - Do we need concurrency? Multi-threading?
 - Any capacity constraints I should model?"

Step 2 — Entities (3 min):
"Let me identify the main entities:
 - Core objects: [list nouns from requirements]
 - Relationships: [has-a, is-a, uses-a]
 - Enums: [states, types]"

Step 3 — APIs first (2 min):
"Let me define the key interfaces before implementing:
 class ParkingLot:
   def park(vehicle) -> Ticket
   def exit(ticket_id) -> float
   def available_spots() -> dict"

Step 4 — Implement core (15 min):
"I'll implement from inner classes outward..."

Step 5 — Extend (5 min):
"To add [new feature], I'd use [design pattern] because..."
```

### 🔑 Design Pattern Quick Reference

| Problem | Pattern | When |
|---------|---------|------|
| Different fee structures | Strategy | Multiple interchangeable algorithms |
| State-based behavior | State | Object behavior changes with state |
| Creating objects | Factory | Decouple creation from usage |
| One instance | Singleton | Config, connection pool |
| Add features without subclassing | Decorator | File I/O, logging |
| Notify multiple objects | Observer | Event systems |
| Encapsulate requests | Command | Undo/redo, queuing |
| Common algorithm, variable steps | Template Method | Data processing pipelines |

### 🏆 Thread Safety Patterns

```python
# Pattern 1: Lock-based (simple, may cause contention)
class ThreadSafeCounter:
    def __init__(self):
        self._count = 0
        self._lock = threading.Lock()
    
    def increment(self):
        with self._lock:  # Context manager ensures unlock
            self._count += 1

# Pattern 2: Per-resource locking (better concurrency)
class ParkingSpotSafe:
    def __init__(self):
        self._lock = threading.Lock()  # One lock per spot
    
    def park(self, vehicle):
        with self._lock:  # Only blocks if THIS spot is contested
            ...

# Pattern 3: Read-write lock (readers don't block each other)
import threading

class ReadWriteLock:
    def __init__(self):
        self._read_ready = threading.Condition(threading.RLock())
        self._readers = 0
    
    def read_acquire(self):
        self._read_ready.acquire()
        self._readers += 1
        self._read_ready.release()
    
    def read_release(self):
        self._read_ready.acquire()
        self._readers -= 1
        if self._readers == 0:
            self._read_ready.notify_all()
        self._read_ready.release()
```

---

*LLD success comes from three things: strong OOP instincts, knowing when to apply design patterns, and clear communication about trade-offs. Practice Parking Lot and Elevator first — they appear most frequently and cover the widest range of OOP concepts.*
