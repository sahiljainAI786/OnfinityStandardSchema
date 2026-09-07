#!/usr/bin/env python3

import os
import stat
import sys


def fix_permissions(root):
    root = os.path.abspath(root)

    if not os.path.isdir(root):
        print(f"ERROR: Folder does not exist: {root}")
        return

    files_changed = 0
    dirs_changed = 0
    skipped = 0

    print(f"\nScanning: {root}\n")

    for current_root, dirs, files in os.walk(root, followlinks=False):

        # Fix current directory
        try:
            old_mode = stat.S_IMODE(os.stat(current_root).st_mode)

            # Directory: rwx for owner, rx for group/others
            os.chmod(current_root, 0o755)

            if old_mode != 0o755:
                dirs_changed += 1
                print(f"[DIR ] {current_root}: {oct(old_mode)} -> 0755")

        except PermissionError:
            skipped += 1
            print(f"[SKIP] {current_root} (Permission denied)")
        except Exception as e:
            skipped += 1
            print(f"[SKIP] {current_root}: {e}")

        # Fix files
        for filename in files:
            path = os.path.join(current_root, filename)

            # Don't modify symlinks
            if os.path.islink(path):
                print(f"[LINK] {path} (skipped)")
                continue

            try:
                old_mode = stat.S_IMODE(os.stat(path).st_mode)

                # File: rw for owner, r for group/others
                os.chmod(path, 0o644)

                if old_mode != 0o644:
                    files_changed += 1
                    print(f"[FILE] {path}: {oct(old_mode)} -> 0644")

            except PermissionError:
                skipped += 1
                print(f"[SKIP] {path} (Permission denied)")
            except Exception as e:
                skipped += 1
                print(f"[SKIP] {path}: {e}")

    print("\n================================")
    print("Permission fixing completed")
    print("================================")
    print(f"Directories changed : {dirs_changed}")
    print(f"Files changed       : {files_changed}")
    print(f"Skipped             : {skipped}")


if __name__ == "__main__":

    # if len(sys.argv) != 2:
    #     print("Usage:")
    #     print("  python3 fix_permissions.py /path/to/folder")
    #     sys.exit(1)

    path = "/home/ca/github_push/StandardSystem"
    fix_permissions(path)