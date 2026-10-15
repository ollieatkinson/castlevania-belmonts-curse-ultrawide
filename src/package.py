#!/usr/bin/env python3
"""Package the INI files with verified, unmodified upstream runtime binaries."""

import hashlib
import io
import urllib.request
import zipfile
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
VERSION = "1.0.0"
RUNTIMES = [
    (
        "https://github.com/PhantomGamers/SUWSF/releases/download/2.3.0/SUWSF-x64.zip",
        "2f0831cba476f014819282e92b870156dbf964c8cc6d6412cd68040bee1612a0",
        "SUWSF.asi",
        "SUWSF.asi",
    ),
    (
        "https://github.com/ThirteenAG/Ultimate-ASI-Loader/releases/download/v9.7.4/Ultimate-ASI-Loader-NoPDB_x64.zip",
        "e5860e7d9a1805267535b65749575b5e406cc6ea3325c7392189c578815045d1",
        "dinput8.dll",
        "version.dll",
    ),
]


def package():
    files = {p.name: p.read_bytes() for p in (ROOT / "src").glob("*.ini")}
    for url, expected, member, name in RUNTIMES:
        archive = urllib.request.urlopen(url, timeout=60).read()
        if hashlib.sha256(archive).hexdigest() != expected:
            raise ValueError("Upstream archive checksum mismatch: " + url)
        with zipfile.ZipFile(io.BytesIO(archive)) as source:
            files[name] = source.read(member)
    for name in ["README.md", "LICENSE", "THIRD_PARTY_NOTICES.md"]:
        files["BelmontsCurseUltrawide/" + name] = (ROOT / name).read_bytes()
    for path in sorted((ROOT / "licenses").glob("*.txt")):
        files["BelmontsCurseUltrawide/licenses/" + path.name] = path.read_bytes()

    output = ROOT / "dist" / ("BelmontsCurseUltrawide-v" + VERSION + ".zip")
    output.parent.mkdir(exist_ok=True)
    with zipfile.ZipFile(output, "w", zipfile.ZIP_DEFLATED, compresslevel=9) as archive:
        for name, data in sorted(files.items()):
            # Keep archive metadata stable across builds.
            entry = zipfile.ZipInfo(name, (2026, 1, 1, 0, 0, 0))
            entry.create_system = 3
            entry.compress_type = zipfile.ZIP_DEFLATED
            entry.external_attr = 0o100644 << 16
            archive.writestr(entry, data, compresslevel=9)
    with zipfile.ZipFile(output) as archive:
        if archive.testzip():
            raise ValueError("ZIP verification failed")
    checksum = hashlib.sha256(output.read_bytes()).hexdigest()
    (output.parent / "SHA256SUMS.txt").write_text(checksum + "  " + output.name + "\n")
    print(output.name, output.stat().st_size, "bytes")


if __name__ == "__main__":
    package()
