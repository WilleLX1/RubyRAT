import os
import platform
import random
import string
import time
import sys
import argparse

if platform.system().startswith("Windows"):
    try:
        from pystyle import *
    except ImportError:
        os.system("python -m pip install pystyle -q -q -q")
        from pystyle import *
elif platform.system().startswith("Linux"):
    try:
        from pystyle import *
    except ImportError:
        os.system("python3 -m pip install pystyle -q -q -q")
        from pystyle import *

banner = Center.XCenter(r"""
# ... (Python encryptor made by Machine1337) ...
""")

value1 = 1
value2 = 100

hexnum = random.randint(value1, value2)

os.system('cls' if os.name == 'nt' else 'clear')
print(Colorate.Vertical(Colors.red_to_purple, banner, 2))

# Create an ArgumentParser object to handle command line arguments
parser = argparse.ArgumentParser(description="Encrypt Python code and specify the output file type.")
parser.add_argument("file_to_encrypt", help="The file to encrypt")
parser.add_argument("output_type", choices=["true", "false"], help="Specify 'true' for .py output or 'false' for .pyw output")

# Parse the command line arguments
args = parser.parse_args()

input_file = args.file_to_encrypt

if not os.path.exists(input_file):
    print(Colors.red + "\n[*] Payload File Not Exists")
    exit()

with open(input_file, "r") as f:
    original_code = f.read()

varname = ''.join(random.choice(string.ascii_lowercase) for _ in range(10))
print(Colors.yellow + "\n[*] File Validation Success!")
time.sleep(1)

def encode_text(text, key):
    encoded_text = []
    for character in text:
        encoded_character = ord(character) + key
        encoded_text.append(f"{encoded_character}")
    return ', '.join(encoded_text)

print(Colors.blue + "\n[*] File Encryption Started....!")
semicode = encode_text(original_code, hexnum)
time.sleep(1)
junk_lines = [''.join(random.choice(string.ascii_lowercase + string.digits) for _ in range(50)) for _ in range(10)]
junk_code = '\n'.join(['# ' + line for line in junk_lines])

# Determine the output file extension based on the user's choice
if args.output_type == "true":
    output_extension = ".py"
else:
    output_extension = ".pyw"

output_file = f"build\\output{output_extension}"

with open(output_file, "w") as f:
    f.write(f"\n#{junk_code}\n")
    f.write(f"{varname} = [")
    f.write(semicode)
    f.write("]\n")
    f.write(f"{varname} = ''.join([chr(int(x) - {hexnum}) for x in {varname}])\n")
    f.write(f"\n#{junk_code}\n")
    f.write(f"exec({varname})\n")
    f.write(f"\n#{junk_code}\n")

print(Colorate.Vertical(Colors.green_to_yellow, f"\n[*] File Successfully Encrypted as {output_file}", 2))
