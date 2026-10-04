# Copilot instructions

## Line endings

- Use CRLF line endings as defined in `.editorconfig`.
- Preserve the line-ending style of every edited file and never introduce mixed line endings.
- Before handoff, verify that every touched text file uses one consistent line-ending style and matches `.editorconfig`.
- Do not create line-ending-only diffs unless explicitly requested.

## Public comments and documentation

- Treat source comments, XML documentation, READMEs, examples and change descriptions as public.
- Do not disclose real personal names, including public maintainers, or private customer/company, project/application or tenant names. Omit internal paths, domains and live customer data.
- Use generic references such as "consumer application", "health endpoint" or "maintainer", and synthetic example data. Preserve technical contracts and actual validation results.
- Public API provider, product and library names may be used where needed to describe the integration. Preserve existing license and copyright attribution.
- Apply this rule to future edits and remove identifying context from comments/documentation when encountered.