# MattEland.Analyzers
A set of custom Rosyln Analyzers for use in my own projects and for learning more about Roslyn analyzers.

This is not intended to be a serious library at the moment, but if these analyzers, code fixes, or refactorings and their tests help you, let me know.

## Included Analyzers & Code Fixes

### ME1001 - Override ToString (Code Fix Available)
This is not a serious analyzer, but rather a proof of concept for analyzers in general. This suggests overriding ToString for classes if you haven't yet.

This could potentially be helpful for debugging or diagnostic purposes.

## Included Refactorings

### Make Public

This makes a type public that was not public before.