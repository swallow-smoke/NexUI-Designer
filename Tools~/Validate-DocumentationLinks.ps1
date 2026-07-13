param(
    [string]$Root = (Split-Path -Parent $PSScriptRoot)
)

$ErrorActionPreference = 'Stop'
$excluded = @('Library', 'Temp', 'Logs', 'obj', '.git')
$rootPrefix = $Root.TrimEnd('\', '/') + [IO.Path]::DirectorySeparatorChar
$markdownFiles = Get-ChildItem -LiteralPath $Root -Filter '*.md' -File -Recurse | Where-Object {
    $relative = if ($_.FullName.StartsWith($rootPrefix, [StringComparison]::OrdinalIgnoreCase)) {
        $_.FullName.Substring($rootPrefix.Length)
    } else { $_.FullName }
    -not ($excluded | Where-Object { $relative -eq $_ -or $relative.StartsWith($_ + [IO.Path]::DirectorySeparatorChar) })
}

function Get-Anchor([string]$Heading) {
    $value = $Heading.Trim().ToLowerInvariant()
    $value = [regex]::Replace($value, '[`*_~]', '')
    $value = [regex]::Replace($value, '[^\p{L}\p{N}\s-]', '')
    return [regex]::Replace($value, '\s+', '-')
}

function Test-ExactCase([string]$Path) {
    $item = Get-Item -LiteralPath $Path -ErrorAction SilentlyContinue
    if ($null -eq $item) { return $false }
    return $item.Name -ceq [IO.Path]::GetFileName($Path)
}

$errors = [Collections.Generic.List[string]]::new()
$linkCount = 0
$pattern = '!?(?<!\!)\[[^\]]*\]\((?<target>[^)]+)\)'

foreach ($file in $markdownFiles) {
    $text = Get-Content -LiteralPath $file.FullName -Raw -Encoding utf8
    foreach ($match in [regex]::Matches($text, $pattern)) {
        $linkCount++
        $target = $match.Groups['target'].Value.Trim().Trim('<', '>')
        if ([string]::IsNullOrWhiteSpace($target)) {
            $errors.Add("EMPTY: $($file.FullName)")
            continue
        }
        if ($target -match '^(https?://|mailto:)' ) { continue }

        $parts = $target.Split('#', 2)
        $pathPart = [Uri]::UnescapeDataString($parts[0])
        $fragment = if ($parts.Count -gt 1) { [Uri]::UnescapeDataString($parts[1]) } else { '' }
        $resolved = if ([string]::IsNullOrEmpty($pathPart)) { $file.FullName } else {
            [IO.Path]::GetFullPath((Join-Path $file.DirectoryName $pathPart))
        }

        if (-not (Test-Path -LiteralPath $resolved)) {
            $errors.Add("MISSING: $($file.FullName) -> $target")
            continue
        }
        if (-not (Test-ExactCase $resolved)) {
            $errors.Add("CASE: $($file.FullName) -> $target")
        }
        if ($fragment -and [IO.Path]::GetExtension($resolved) -ieq '.md') {
            $targetText = Get-Content -LiteralPath $resolved -Encoding utf8
            $anchors = @($targetText |
                Where-Object { $_ -match '^#{1,6}\s+(.+?)\s*$' } |
                ForEach-Object { Get-Anchor $Matches[1] })
            $anchors += @($targetText |
                Select-String -Pattern '<a\s+(?:name|id)=["'']([^"'']+)["'']' -AllMatches |
                ForEach-Object { $_.Matches.Groups[1].Value.ToLowerInvariant() })
            if ($anchors -cnotcontains $fragment.ToLowerInvariant()) {
                $errors.Add("ANCHOR: $($file.FullName) -> $target")
            }
        }
    }
}

Write-Output "Markdown files: $($markdownFiles.Count)"
Write-Output "Relative links: $linkCount"
Write-Output "Errors: $($errors.Count)"
$errors | ForEach-Object { Write-Output $_ }
if ($errors.Count -gt 0) { exit 1 }
