# GamelistMerger

A command-line tool for merging and deduplicating `gamelist.xml` files used by emulation frontends such as EmulationStation, Batocera, and RetroPie.

<img width="950" height="822" alt="image" src="https://github.com/user-attachments/assets/4fe8840c-bf6a-4702-9fec-26ce68c9045b" />


## What It Does

GamelistMerger takes two `gamelist.xml` files, combines them into one, removes duplicate entries (based on ROM file path), and optionally filters out unwanted games.

This is useful when you have game metadata from multiple sources and want to consolidate them into a single gamelist.xml file.

## Usage

```
GamelistMerger -a <file> -b <file> -o <output> [options]
```

### Required Arguments

| Argument | Description |
|----------|-------------|
| `-a, --gamelistA <file>` | Path to the first gamelist.xml file (master) |
| `-b, --gamelistB <file>` | Path to the second gamelist.xml file |
| `-o, --output <file>` | Path for the merged output file |

### Filter Options

| Option | Description |
|--------|-------------|
| `--exclude-bios` | Exclude BIOS files (games with 'BIOS' in name) |
| `--exclude-name-contains <values>` | Exclude games where name contains any of these (comma-separated) |
| `--exclude-region <regions>` | Exclude games from these regions (e.g., `jp,eu`) |
| `--include-lang <languages>` | Only include games with these languages (e.g., `en,jp`) |

### Output Options

| Option | Description |
|--------|-------------|
| `--prefer-archives` | Prefer the .zip/.7z file if available |
| `--overwrite` | Overwrite output file if it already exists |
| `-v, --verbose` | Show paths of filtered games |
| `--sortOutput` | Sort games by name in the output file |

## I want to:

**Merge two gamelists and remove duplicates:**

```bash
GamelistMerger -a gamelistA.xml -b gamelistB.xml -o merged.xml
```

**Merge gamelists and overwrite an existing output file:**

```bash
GamelistMerger -a gamelistA.xml -b gamelistB.xml -o merged.xml --overwrite
```

**Merge gamelists and sort the output alphabetically by game name:**

```bash
GamelistMerger -a gamelistA.xml -b gamelistB.xml -o merged.xml --sortOutput
```

**Prefer archived ROMs (.zip/.7z) over uncompressed files:**

```bash
GamelistMerger -a gamelistA.xml -b gamelistB.xml -o merged.xml --prefer-archives
```

**Exclude BIOS files from the merged list:**

```bash
GamelistMerger -a gamelistA.xml -b gamelistB.xml -o merged.xml --exclude-bios
```

**Keep only English games:**

```bash
GamelistMerger -a gamelistA.xml -b gamelistB.xml -o merged.xml --include-lang en
```

**Keep only English and Japanese games:**

```bash
GamelistMerger -a gamelistA.xml -b gamelistB.xml -o merged.xml --include-lang en,jp
```

**Exclude Japanese and European region games:**

```bash
GamelistMerger -a gamelistA.xml -b gamelistB.xml -o merged.xml --exclude-region jp,eu
```

**Exclude games with certain words in their name (e.g., demos, prototypes):**

```bash
GamelistMerger -a gamelistA.xml -b gamelistB.xml -o merged.xml --exclude-name-contains "Demo,Proto,Beta"
```

**See which games were filtered out:**

```bash
GamelistMerger -a gamelistA.xml -b gamelistB.xml -o merged.xml --exclude-bios --verbose
```

**Create a filtered English-only collection with sorted output:**

```bash
GamelistMerger -a gamelistA.xml -b gamelistB.xml -o merged.xml --exclude-bios --include-lang en --exclude-name-contains "Demo,Proto,Beta" --sortOutput --overwrite
```

## How Deduplication Works

Games are considered duplicates if they match on any of these properties (checked in order):

1. `<hash>` - ROM file hash
2. `<crc32>` - CRC32 checksum
3. `<path>` - ROM file path

When duplicates are found, the entries are **merged** rather than simply discarded. For each field, the first file (`-a`) takes priority, but if a field is empty in the first file, the value from the second file (`-b`) is used. This tries to produces the most comprehensive metadata possible for each game.



