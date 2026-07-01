# RAG and Prompt Engineering

## Part 1: Retrieval-Augmented Generation (RAG)

### What is RAG?

**RAG** combines the power of LLMs with external knowledge retrieval to:
- Reduce hallucinations
- Provide up-to-date information
- Ground responses in specific documents
- Enable domain-specific applications

### The RAG Problem

**LLM Limitations:**
- Knowledge cutoff date
- Cannot access private/proprietary data
- May hallucinate facts
- No real-time information

**RAG Solution:**
- Retrieve relevant documents from a knowledge base
- Provide them as context to the LLM
- LLM generates answer based on retrieved information

### RAG Architecture

```
User Query
    ↓
Query Embedding (convert to vector)
    ↓
Vector Search (find similar documents)
    ↓
Retrieve Top-K Documents
    ↓
Combine Query + Retrieved Context
    ↓
Send to LLM
    ↓
Generate Response
```

### RAG Components

#### 1. Document Ingestion
```
Documents → Split into Chunks → Generate Embeddings → Store in Vector DB
```

**Chunking Strategies:**
- Fixed size (e.g., 512 tokens)
- Semantic chunking (by paragraph/section)
- Sliding window with overlap
- Recursive splitting

**Considerations:**
- Chunk size: Balance between context and specificity
- Overlap: Prevent losing context at boundaries
- Metadata: Store source, page number, timestamps

#### 2. Embedding Models

Convert text into vector representations:

**Popular Options:**
- OpenAI embeddings (ada-002)
- Sentence Transformers (open-source)
- Cohere embeddings
- Custom fine-tuned models

**Key Metrics:**
- Dimension size (768, 1536, etc.)
- Performance on retrieval tasks
- Cost and latency

#### 3. Vector Databases

Store and search embeddings efficiently:

**Popular Options:**
- Pinecone (managed)
- Weaviate (open-source)
- Qdrant (open-source)
- Chroma (lightweight)
- FAISS (library)
- Milvus (scalable)

**Search Methods:**
- Cosine similarity
- Dot product
- Euclidean distance

#### 4. Retrieval Strategies

**Semantic Search:**
- Find documents similar to query embedding
- Most common RAG approach

**Hybrid Search:**
- Combine semantic + keyword search (BM25)
- Better for exact matches and technical terms

**Reranking:**
- Initial retrieval (fast but less accurate)
- Rerank with more sophisticated model
- Return top results

**Query Expansion:**
- Generate multiple variations of query
- Retrieve for each variation
- Combine results

#### 5. Context Assembly

**Strategies:**
- Concatenate all retrieved chunks
- Summarize retrieved content first
- Use only most relevant snippets
- Include metadata (source, confidence)

**Challenges:**
- Token limits
- Relevant information ordering
- Conflicting information

### Advanced RAG Techniques

#### Multi-Query RAG
- Generate multiple queries from user input
- Retrieve for each query
- Combine results

#### Hypothetical Document Embeddings (HyDE)
- Generate hypothetical answer to query
- Embed the hypothetical answer
- Search for similar documents
- Often more effective than searching with query directly

#### Self-RAG
- Model decides when to retrieve
- Can retrieve multiple times during generation
- More dynamic and flexible

#### Agentic RAG
- AI agent decides:
  - When to retrieve
  - What to search for
  - How to combine information
- Multiple retrieval rounds
- Can use tools and APIs

### RAG Evaluation

**Metrics:**
- **Retrieval Quality**:
  - Precision@K
  - Recall@K
  - MRR (Mean Reciprocal Rank)
  - NDCG (Normalized Discounted Cumulative Gain)

- **Generation Quality**:
  - Faithfulness (answer grounded in context?)
  - Answer relevance
  - Context relevance
  - Human evaluation

**Tools:**
- RAGAS (RAG Assessment)
- TruLens
- LangSmith evaluation

### RAG Best Practices

1. ✅ **Chunk carefully**: Size matters for retrieval quality
2. ✅ **Use metadata**: Add filters for better retrieval
3. ✅ **Include citations**: Show sources in responses
4. ✅ **Handle no-results**: When retrieval finds nothing relevant
5. ✅ **Monitor performance**: Track retrieval and generation quality
6. ✅ **Consider hybrid search**: Combine semantic + keyword
7. ✅ **Rerank when possible**: Improve top results quality

---

## Part 2: Prompt Engineering

### What is Prompt Engineering?

The art and science of crafting inputs to LLMs to get desired outputs.

### Why Prompt Engineering Matters

- Same model, different prompts = vastly different results
- Well-crafted prompts can improve accuracy by 30-50%
- Cost-effective way to improve performance
- Faster than fine-tuning

### Core Principles

#### 1. Be Clear and Specific

❌ **Bad:**
```
Write about dogs.
```

✅ **Good:**
```
Write a 200-word article about the benefits of adopting rescue dogs, 
targeting first-time dog owners. Include sections on cost savings, 
emotional rewards, and practical considerations.
```

#### 2. Provide Context

❌ **Bad:**
```
Summarize this.
```

✅ **Good:**
```
You are a technical writer creating documentation for developers.
Summarize the following API documentation into a quick-start guide 
with code examples. Target audience: junior developers.

[API documentation here]
```

#### 3. Specify Format

❌ **Bad:**
```
List some project ideas.
```

✅ **Good:**
```
Provide 5 project ideas in the following JSON format:
{
  "title": "project name",
  "difficulty": "beginner|intermediate|advanced",
  "technologies": ["tech1", "tech2"],
  "description": "brief description"
}
```

### Key Techniques

#### 1. Zero-Shot Prompting

Ask directly without examples:
```
Classify the sentiment of this review as positive, negative, or neutral:
"The product works but customer service was terrible."
```

#### 2. Few-Shot Prompting

Provide examples:
```
Classify sentiment (positive/negative/neutral):

Example 1:
Review: "Best purchase ever!"
Sentiment: positive

Example 2:
Review: "Waste of money"
Sentiment: negative

Example 3:
Review: "The product works but customer service was terrible."
Sentiment: ?
```

#### 3. Chain-of-Thought (CoT)

Ask model to reason step-by-step:
```
Solve this problem step by step:

A store offers a 20% discount on a $50 item. After the discount, 
there's an additional 10% off. What's the final price?

Let's solve this step by step:
1. First discount:
2. Price after first discount:
3. Second discount:
4. Final price:
```

#### 4. Self-Consistency

Generate multiple reasoning paths:
```
Generate 3 different solutions to this problem, then identify 
the most likely correct answer.
```

#### 5. Role-Based Prompting

Assign a role:
```
You are an expert Python developer with 10 years of experience 
in data engineering. Review this code and suggest improvements 
focusing on performance and maintainability.
```

#### 6. Instruction Following

Be explicit about constraints:
```
Answer in exactly 50 words. Do not use jargon. Explain as if 
talking to a 12-year-old.
```

### Advanced Techniques

#### ReAct (Reasoning + Acting)

Combine reasoning with tool use:
```
Answer this question using available tools.

Available tools:
- search(query): Search the internet
- calculate(expression): Perform calculation

Think step by step:
1. What information do I need?
2. Which tool should I use?
3. What is the result?
4. What is my final answer?
```

#### Tree of Thoughts

Explore multiple reasoning paths:
```
Problem: [complex problem]

Generate 3 possible approaches:
Approach 1: [...]
Approach 2: [...]
Approach 3: [...]

Evaluate each approach:
[evaluation]

Select best approach and solve:
[solution]
```

#### Retrieval-Augmented Prompting

Combine with external knowledge:
```
Context: [retrieved documents]

Based only on the above context, answer: [question]

If the context doesn't contain the answer, say "I don't have 
enough information to answer this question."
```

### Common Patterns

#### System-User-Assistant Pattern

```
System: You are a helpful assistant that answers questions concisely.

User: What is machine learning?
