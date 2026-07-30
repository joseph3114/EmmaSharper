# Wiki source

These files are the **source of truth** for the
[GitHub wiki](https://github.com/joseph3114/EmmaSharper/wiki).

They live in the repository rather than only in the wiki so that they are versioned with the code,
reviewed in pull requests, and travel with a source download. A GitHub wiki is a separate git
repository with no review step, which makes it easy for documentation to drift away from the
library it describes.

## API-Reference.md is generated

Do not hand-edit it. It is produced from the compiled assembly and its XML documentation, so
signatures, nullability and summaries come from the code rather than from someone remembering to
update a page. Regenerate after any public API change:

```bash
./tools/gen-api-reference.sh
```

To change how it reads, edit the XML doc comments in the source or the generator itself
(`tools/gen-api-reference.cs`).

## Publishing

The wiki repository does not exist until the first page is created through the web UI. Once it
does, mirror this directory into it:

```bash
git clone https://github.com/joseph3114/EmmaSharper.wiki.git /tmp/emmasharper.wiki
cp docs/wiki/*.md /tmp/emmasharper.wiki/
cd /tmp/emmasharper.wiki && git add -A && git commit -m "Sync wiki from docs/wiki" && git push
```

File names map to page titles, with hyphens becoming spaces. `_Sidebar.md` is the navigation
panel and is not itself a page.

## Editing

Change the files here and open a pull request, then re-run the mirror. Editing pages directly in
the wiki UI will be overwritten on the next sync.
