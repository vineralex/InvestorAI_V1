---
type: policy
title: Knowledge review checklist
description: Checks required after changes to the knowledge bundle.
tags: [knowledge, maintenance, review]
updated_at: 2026-10-09
status: approved
---

# Knowledge review checklist

Review the affected branches after each knowledge change. After restructuring
the bundle, check the entire navigation tree and affected external references.

- [ ] Files have focused topics and are split by meaning, not line count.
- [ ] Indexes route to the relevant topics without requiring the whole bundle
  to be read. Every document is reachable through the index tree.
- [ ] Historical evidence is separate from current status, plans, and decisions.
- [ ] Approved decisions, intentions, and unresolved questions remain distinct;
  completion claims match reported evidence.
- [ ] Local links and anchors resolve, including references from README and docs.
- [ ] UTF-8 Markdown and OKF metadata are valid and current; unknown frontmatter
  fields are preserved.
- [ ] Solution Explorer includes the knowledge files at their correct paths.
- [ ] Content is preserved without unnecessary duplication or temporary files.

Record any unresolved findings when reporting the review. A file's size alone
is neither a failure nor proof that its structure is sound.

[Back to maintenance index](index.md)
