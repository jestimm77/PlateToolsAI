# PlateToolsAI

PlateToolsAI is a WinForms cut-list routing tool for importing steel plate cut lists, reviewing part rows, applying optional AI machine suggestions, manually correcting routes, and checking quantity-based totals.

## Importing a cut list

1. Open the app and click **Process Job**.
2. Select a cut-list file:
   - **PDF** for text-backed cut lists.
   - **PDF** plus OCR tools in `PATH` (`pdftoppm` and `tesseract`) for scanned/image PDFs.
   - **TXT/CSV** if you already have exported cut-list text.
3. Review the imported parts grid. Duplicate piece marks are kept as separate rows and totals use **quantities**, not row counts.

## AI machine suggestions

- Turn **Use AI machine suggestions when available** on to apply recognized `Piece Mark -> Machine` matches.
- Supported routing values are:
  - `Shear`
  - `FPB`
  - `2500A`
  - `2500B`
  - `Burn Table`
  - `ESAB-1`
  - `ESAB-2`
  - `ESAB-3`
  - `T-Order`
  - `Stock`
- If a machine is missing, unsupported, or ambiguous, the row stays available for manual review instead of being guessed.

## Manual corrections and totals

- The **Machine** column stays editable so you can correct or complete routing assignments.
- Use the **Available Lots** and **Available Sequences** filters to narrow the current view.
- The totals panel shows:
  - overall project quantity
  - visible filtered quantity
  - quantity still needing review
  - per-machine project totals
  - filtered totals for the current view

## Notes

- For scanned PDFs, install Poppler (`pdftoppm`) and Tesseract (`tesseract`) so PlateToolsAI can OCR the pages when a PDF has no usable text layer.
- If OCR or machine recognition is incomplete, use the row status values and totals summary to finish routing manually without losing imported quantities.
