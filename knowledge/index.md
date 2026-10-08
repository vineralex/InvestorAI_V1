---
okf_version: "0.2"
title: Investor AI knowledge
description: Entry point for the Investor AI Open Knowledge Format bundle.
---

# Start here

This directory is an Open Knowledge Format (OKF) v0.2 bundle. When creating or
editing knowledge:

- use UTF-8 Markdown with YAML frontmatter and follow the
  [OKF specification](https://github.com/GoogleCloudPlatform/open-knowledge-format/blob/main/SPEC.md);
- give every concept document a non-empty `type` and provide a concise `title`,
  `description`, relevant `tags`, `updated_at`, and lifecycle `status`;
- use `index.md` files for progressive disclosure and standard Markdown links
  between related concepts;
- preserve unknown OKF frontmatter fields when editing existing documents.

Follow the relevant index branch, then read only the leaf documents needed.
Do not recursively load every linked document or read the whole bundle.
Keep indexes short and move independent topics and historical evidence into
separate files. Prefer one focused topic per leaf; split by meaning, not a
fixed line count. Cross-links supplement the tree and are followed only when
the current task requires them. Human setup instructions belong in README/docs.

Documents marked
as `status: intention` describe direction to investigate, not approved
architecture.

- Product value, scope, and success: [Product](product/index.md)
- Architecture, identity, isolation, and implementation conventions: [Architecture](architecture/index.md)
- Current delivery plan, slice contracts, and verification evidence: [Vertical slices](vertical-slices/index.md)
