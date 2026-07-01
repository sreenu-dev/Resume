# Transformer Architecture Deep Dive

## Introduction

The **Transformer** is a neural network architecture introduced by Google in a landmark 2017 paper ("Attention Is All You Need"). It revolutionized natural language processing and is the foundation of all modern LLMs (GPT, Claude, LLaMA, etc.).

## Why Transformers?

### Problems with Previous Architectures

**Recurrent Neural Networks (RNNs):**
- ❌ Sequential processing (slow)
- ❌ Difficulty with long-range dependencies
- ❌ Cannot utilize GPU parallelism effectively
- ❌ Vanishing gradient problems

**Transformers Solve These:**
- ✅ Parallel processing of entire sequences
- ✅ Excellent long-range dependency handling
- ✅ Highly parallelizable (fast training on GPUs)
- ✅ Stable gradient flow

## Transformer Components

### High-Level Structure

A full transformer consists of:

1. **Encoder**: Converts input text into an intermediate representation
2. **Decoder**: Converts that representation into useful text output

Both encoder and decoder are enormous neural networks.

### Three Main Transformer Types

#### 1. Encoder-Only (e.g., BERT)
- Maps input text into embeddings
- **Use cases**:
  - Text classification
  - Named entity recognition
  - Sentiment analysis
  - Creating embeddings for retrieval

#### 2. Decoder-Only (e.g., GPT, Claude, LLaMA)
- Generates text autoregressively
- **Use cases**:
  - Text generation
  - Chatbots
  - Code generation
  - Creative writing

#### 3. Encoder-Decoder (e.g., T5, BART)
- Full encoder + decoder
- **Use cases**:
  - Translation
  - Summarization
  - Question answering with context

## Core Mechanism: Self-Attention

Self-attention is the **key innovation** that makes transformers powerful.

### What is Self-Attention?

Self-attention allows each word to "look around" at other words in the sequence and gather relevant context.

**Example:**
> "The animal didn't cross the street because **it** was too tired."

- What does "it" refer to?
- Self-attention helps the model determine that "it" refers to "animal" (not "street")
- It examines relationships between all words simultaneously

### How Self-Attention Works

Think of self-attention as a **matchmaking service for words**:

1. **Query Vector**: Each word creates a checklist describing what information it's looking for
2. **Key Vector**: Each word creates a checklist describing its own characteristics
3. **Value Vector**: The actual information the word will share

**Process:**
```
1. Compare each Query with all Keys (compute dot products)
2. Find the best matches (attention scores)
3. Transfer information from matching words (weighted sum of Values)
4. Each word updates its representation based on relevant context
```

### Bidirectional vs. Unidirectional Attention

**Bidirectional (used in encoders):**
- Can see words on both sides
- Gathers context from past and future
- Best for understanding complete sequences
- Example: BERT

**Unidirectional/Causal (used in decoders):**
- Can only see previous words
- Prevents "cheating" during generation
- Essential for text generation
- Example: GPT models

## Multi-Head Attention

Instead of one attention mechanism, transformers use **multiple attention heads** in parallel.

### Why Multiple Heads?

Different heads can learn different types of relationships:
- Head 1: Syntactic relationships (subject-verb agreement)
- Head 2: Semantic relationships (synonyms, antonyms)
- Head 3: Long-range dependencies
- Head 4: Local context
- etc.

**Typical Configuration:**
- GPT-3: 96 attention heads per layer
- 96 layers total
- Each head learns different patterns

## The Feed-Forward Network

After self-attention, each word vector passes through a **feed-forward network**:

1. Takes the enriched word representation (post-attention)
2. Processes it through 2-3 dense layers
3. Adds non-linearity (helps with complex patterns)
4. Helps predict the next token

**This happens in every layer of the transformer.**

## Positional Encoding

### The Problem

Self-attention treats the sequence as a **set**, not a sequence:
- Word order information is lost
- "Dog bites man" vs. "Man bites dog" would look the same

### The Solution: Positional Encodings

Add position information to each word's embedding:
- Mathematical function (sine/cosine) or learned embeddings
- Encodes the absolute or relative position
- Allows model to understand word order

## Layer Organization

Modern LLMs stack many transformer layers:

```
Input Text
   ↓
Tokenization
   ↓
Embeddings + Positional Encoding
   ↓
[Layer 1: Multi-Head Attention → Feed-Forward]
   ↓
[Layer 2: Multi-Head Attention → Feed-Forward]
   ↓
[Layer 3: Multi-Head Attention → Feed-Forward]
   ↓
   ...
   ↓
[Layer N: Multi-Head Attention → Feed-Forward]
   ↓
Output Prediction
```

**Each layer adds information to help:**
- Clarify word meanings
- Build context
- Predict the next token

### Example Layer Counts

- GPT-2: 12-48 layers
- GPT-3: 96 layers
- GPT-4: Unknown (estimated 120+ layers)
- LLaMA 2 70B: 80 layers

## Additional Components

### Layer Normalization
- Normalizes activations within each layer
- Stabilizes training
- Improves convergence

### Residual Connections
- Skip connections around each sub-layer
- Helps gradients flow backward
- Prevents vanishing gradients in deep networks

### Dropout
- Randomly drops neurons during training
- Prevents overfitting
- Improves generalization

## How Text Generation Works

1. **Input**: User provides a prompt
2. **Tokenization**: Convert text to tokens
3. **Processing**: Pass through all transformer layers
4. **Prediction**: Model outputs probability distribution over vocabulary
5. **Sampling**: Select next token (greedy, top-k, nucleus sampling)
6. **Repeat**: Add predicted token to input, generate next token
7. **Continue**: Until stopping condition (max length, end token, etc.)

## Key Parameters

### Temperature
- Controls randomness in generation
- Low (0.1-0.5): More deterministic, focused
- High (0.8-1.5): More creative, diverse

### Top-K Sampling
- Consider only the top K most probable tokens
- Typical values: 40-50

### Top-P (Nucleus) Sampling
- Consider tokens until cumulative probability exceeds P
- Typical values: 0.9-0.95

## Advantages of Transformers

1. ✅ **Parallelization**: Process entire sequences at once
2. ✅ **Long-range dependencies**: Self-attention captures relationships across long distances
3. ✅ **Scalability**: Can be scaled to billions of parameters
4. ✅ **Transfer learning**: Pre-trained models can be fine-tuned for specific tasks
5. ✅ **Flexibility**: Same architecture works for many tasks

## Limitations

1. ❌ **Quadratic complexity**: Attention is O(n²) in sequence length
2. ❌ **Memory intensive**: Storing attention matrices requires significant memory
3. ❌ **Context window**: Limited by memory and compute constraints
4. ❌ **Compute cost**: Training requires massive computational resources

## Modern Optimizations

### Flash Attention
- More efficient attention computation
- Reduces memory usage
- Faster training and inference

### Sparse Attention
- Only attend to subset of tokens
- Reduces complexity
- Examples: Longformer, BigBird

### Rotary Position Embeddings (RoPE)
- Better position encoding
- Used in modern models (LLaMA, GPT-NeoX)

### Group Query Attention (GQA)
- Shares key/value projections across heads
- Reduces memory and compute
- Used in LLaMA 2

## Key Takeaways

1. Transformers use **self-attention** to understand context
2. **Multi-head attention** allows learning different relationship types
3. **Parallel processing** makes transformers fast and efficient
4. **Positional encoding** preserves word order information
5. Modern LLMs stack **many transformer layers** (80-120+)
6. Self-attention is powerful but computationally expensive (O(n²))
7. Recent optimizations make transformers more efficient

## Interview Tips

**Be ready to explain:**
- How self-attention works (Query, Key, Value)
- Difference between encoder and decoder
- Why transformers are better than RNNs
- Multi-head attention purpose
- How positional encoding works
- Computational complexity of attention

## Further Reading

- Original Paper: "Attention Is All You Need" (Vaswani et al., 2017)
- The Illustrated Transformer (Jay Alammar)
- Understanding transformers requires grasping self-attention first
