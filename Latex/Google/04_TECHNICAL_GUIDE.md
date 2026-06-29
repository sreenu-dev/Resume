# Technical Interview Guide — Google (Coding + Massive-Scale System Design)

## Google Coding Interview — What's Different

| Dimension      | Amazon             | JPMC                   | HSBC                   | **Google**                                        |
| ----------------| --------------------| ------------------------| ------------------------| ---------------------------------------------------|
| Language       | Any                | Java preferred         | Any                    | **Python/Java preferred**                         |
| Difficulty     | Medium–Hard        | Medium–Hard            | Medium                 | **Hard**                                          |
| Code style     | Correct + optimal  | Correct + secure       | Correct + maintainable | **Correct + optimal + tested**                    |
| Follow-ups     | "Optimize further" | "What could go wrong?" | "How would you scale?" | **"How would you optimize? How would you test?"** |
| Domain context | None               | Finance                | Banking                | **None (CS fundamentals)**                        |
| Testing        | Rare               | Sometimes              | Sometimes              | **Always**                                        |

---

## Google's Most Frequently Asked Coding Problems

### Tier 1 — High Probability

| # | Problem | Pattern | Google Angle |
|---|---|---|---|
| 1 | Two Sum | HashMap | How would you optimize? |
| 2 | Valid Parentheses | Stack | Can you do it in one pass? |
| 3 | Merge Two Sorted Lists | Linked List | How would you test this? |
| 4 | Binary Tree Level Order Traversal | BFS | How would you handle a billion-node tree? |
| 5 | Number of Islands | DFS/BFS | How would you parallelize this? |
| 6 | LRU Cache | DLL + HashMap | How would you make this thread-safe? |
| 7 | Longest Substring Without Repeating | Sliding Window | What's the optimal space complexity? |
| 8 | Merge Intervals | Sorting | How would you optimize for memory? |
| 9 | Course Schedule | Topological Sort | How would you test this? |
| 10 | Serialize / Deserialize Binary Tree | BFS | How would you handle a petabyte-scale tree? |
| 11 | Find Median from Data Stream | Two Heaps | How would you optimize latency? |
| 12 | Kth Largest Element | Heap | How would you do this in a distributed system? |
| 13 | Clone Graph | BFS + HashMap | How would you handle a trillion-node graph? |
| 14 | Word Ladder | BFS | How would you parallelize this? |
| 15 | Design Rate Limiter | System Design | How would you optimize for latency? |

---

## Massive-Scale System Design — 5 Deep Dives

### Problem 1: Design a Search Engine / Web Crawler

**Context:** Google's core business is search. Your system must crawl and index the entire web.

**Requirements:**
```
Functional:
  - Crawl billions of web pages
  - Index pages for fast search
  - Rank results by relevance
  - Handle updates (new pages, deleted pages)

Non-Functional:
  - 1 billion pages crawled per day
  - Search latency < 100ms (p99)
  - 99.99% availability
  - Handle petabyte-scale data
```

**Architecture (Simplified):**
```
URL Frontier (priority queue of URLs to crawl)
  → Crawler Workers (distributed, 1000s of machines)
      - Fetch page
      - Extract links
      - Send to Indexer
  → Indexer Service:
      - Parse HTML
      - Extract text + metadata
      - Build inverted index
  → Distributed Index (BigTable-like):
      - Sharded by URL hash
      - Replicated for availability
  → Ranking Service:
      - PageRank algorithm
      - User signals (clicks, dwell time)
  → Query Service:
      - Retrieve matching pages
      - Rank by relevance
      - Return in < 100ms
```

**Key Design Points:**
- **Distributed crawling:** 1000s of crawlers working in parallel
- **Inverted index:** map from word → list of pages containing that word
- **PageRank:** algorithm to rank pages by importance
- **Sharding:** distribute index across machines by URL hash
- **Caching:** cache popular queries and results

---

### Problem 2: Design YouTube / Video Streaming at Scale

**Context:** YouTube serves billions of videos to billions of users. Your system must handle massive scale.

**Requirements:**
```
Functional:
  - Upload videos
  - Stream videos to users
  - Search for videos
  - Recommend videos

Non-Functional:
  - 1 billion hours of video watched per day
  - 500 hours of video uploaded per minute
  - < 1 second startup latency
  - 99.99% availability
  - Petabyte-scale storage
```

**Architecture:**
```
Video Upload Service:
  - Accept video upload
  - Transcode to multiple resolutions (480p, 720p, 1080p, 4K)
  - Store in distributed storage (GCS)
  - Update metadata in database

Video Streaming Service:
  - User requests video
  - Determine user's bandwidth
  - Serve appropriate resolution
  - Use CDN for low latency
  - Cache popular videos at edge

Recommendation Service:
  - Collaborative filtering
  - Content-based filtering
  - Real-time ranking
  - A/B testing for new algorithms

Search Service:
  - Inverted index of video metadata
  - Distributed search across shards
  - Ranking by relevance + popularity
```

**Key Design Points:**
- **Transcoding:** convert video to multiple resolutions
- **CDN:** content delivery network for low latency
- **Distributed storage:** store petabytes of video
- **Recommendation:** machine learning to personalize
- **Caching:** cache popular videos at edge

---

### Problem 3: Design Google Maps / Location Services

**Context:** Google Maps serves billions of location queries. Your system must handle real-time updates.

**Requirements:**
```
Functional:
  - Store map data (roads, buildings, POIs)
  - Route finding (shortest path)
  - Real-time traffic updates
  - Geocoding (address → coordinates)

Non-Functional:
  - 1 billion location queries per day
  - Route finding < 500ms
  - Real-time traffic updates
  - 99.99% availability
```

**Architecture:**
```
Map Data Service:
  - Store map graph (nodes = intersections, edges = roads)
  - Partition graph by geographic region
  - Replicate for availability

Route Finding Service:
  - Dijkstra's algorithm (or A*)
  - Pre-computed shortest paths (for popular routes)
  - Caching for frequently requested routes

Traffic Service:
  - Real-time traffic data from users
  - Aggregate traffic signals
  - Update route recommendations
  - Pub/Sub for real-time updates

Geocoding Service:
  - Map address → coordinates
  - Reverse geocoding (coordinates → address)
  - Caching for popular locations
```

**Key Design Points:**
- **Graph partitioning:** divide map into regions
- **Shortest path algorithms:** Dijkstra, A*, pre-computed paths
- **Real-time updates:** Pub/Sub for traffic updates
- **Caching:** cache popular routes and locations
- **Distributed storage:** store petabytes of map data

---

### Problem 4: Design a Distributed Cache (Like Memcached)

**Context:** Google uses distributed caches to serve billions of requests. Your system must handle massive throughput.

**Requirements:**
```
Functional:
  - Store key-value pairs
  - Get/Set operations
  - Eviction policy (LRU)

Non-Functional:
  - 1 million requests per second
  - < 1ms latency (p99)
  - 99.99% availability
  - Terabyte-scale data
```

**Architecture:**
```
Client Library:
  - Hash key to server
  - Send request to server
  - Handle failures (retry, fallback)

Cache Servers (distributed):
  - In-memory hash table
  - LRU eviction
  - Replication for availability
  - Consistent hashing for scaling

Monitoring:
  - Track hit rate
  - Track latency
  - Alert on failures
```

**Key Design Points:**
- **Consistent hashing:** add/remove servers without rehashing all keys
- **Replication:** replicate data for availability
- **LRU eviction:** remove least recently used items when full
- **In-memory storage:** use RAM for sub-millisecond latency
- **Monitoring:** track hit rate and latency

---

### Problem 5: Design a Rate Limiter / API Gateway

**Context:** Google's APIs serve billions of requests. Your system must protect against abuse.

**Requirements:**
```
- Rate limit per user/IP
- Distributed rate limiting (across multiple servers)
- < 10ms overhead per request
- 99.99% availability
```

**Architecture:**
```
API Gateway:
  - Receive request
  - Check rate limit (Redis)
  - If allowed: forward to backend
  - If denied: return 429 Too Many Requests

Rate Limiter (Redis):
  - Token bucket per user
  - Sliding window counter
  - Distributed across Redis cluster
```

**Key Design Points:**
- **Token bucket:** simple, efficient rate limiting
- **Distributed:** use Redis for shared state
- **Low latency:** < 10ms overhead
- **Graceful degradation:** if rate limiter fails, allow requests

---

## Practice Schedule (3 Weeks)

```
Week 1: Coding Fundamentals
  Mon: 5 medium problems in Python (Two Sum, Valid Parentheses, BFS Tree)
  Tue: 5 medium problems (Merge Intervals, Sliding Window)
  Wed: LRU Cache + Design HashMap
  Thu: 5 medium problems (Graphs, Topological Sort)
  Fri: Full OA mock (90 min, 2 problems in Python)
  Sat: Review + fix
  Sun: Rest

Week 2: System Design + Scale Thinking
  Mon: Design Search Engine / Web Crawler (60 min)
  Tue: 5 medium problems (Heaps, Stacks)
  Wed: Design YouTube / Video Streaming (60 min)
  Thu: Design Google Maps / Location Services (60 min)
  Fri: Full coding mock (45 min, 1 problem in Python)
  Sat: Review
  Sun: Rest

Week 3: Mock Interviews + Behavioral
  Mon: Full loop simulation (2 coding + 1 SD)
  Tue: 5 hard problems (Median from Stream, Word Ladder)
  Wed: Design Distributed Cache (60 min)
  Thu: Full behavioral mock (45 min, 5 questions)
  Fri: Design Rate Limiter (60 min)
  Sat: Final review
  Sun: Rest
```

---

**Next: Read `05_BEHAVIORAL_GUIDE.md`**
