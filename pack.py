import json
import shutil
import sys
import zipfile
from pathlib import Path

project = Path(sys.argv[1])
output = Path(sys.argv[2])
mods = Path(sys.argv[3])
configuration = sys.argv[4] if len(sys.argv) > 4 else ""

info = json.loads((project / "modinfo.json").read_text())
zip_name = f"{info['modid']}_{info['version']}.zip"
staging = project / "bin" / zip_name

if staging.exists():
    staging.unlink()

def packed(path: Path) -> bool:
    name = path.name
    if name.endswith(".pdb") or name.endswith(".deps.json"):
        return False
    relative = path.relative_to(output).as_posix()
    return relative in ("modinfo.json", "modicon.png") or relative.endswith(".dll") or relative.startswith("assets/")

with zipfile.ZipFile(staging, "w", compression=zipfile.ZIP_DEFLATED) as archive:
    for path in output.rglob("*"):
        if path.is_file() and packed(path):
            archive.write(path, path.relative_to(output).as_posix())

if configuration.lower() == "release":
    release_dir = project / "Release"
    release_dir.mkdir(parents=True, exist_ok=True)
    for old in release_dir.glob(f"{info['modid']}_*.zip"):
        old.unlink()
    shutil.copy2(staging, release_dir / zip_name)
    print(f"Packed {release_dir / zip_name}")

mods.mkdir(parents=True, exist_ok=True)
for old in mods.glob(f"{info['modid']}_*.zip"):
    old.unlink()

destination = mods / zip_name
shutil.move(staging, destination)
print(f"Moved {destination}")
