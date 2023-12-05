# RubyRAT

RubyRAT is a remote access trojan developed in Python, designed to utilize a Discord bot as a Command and Control (C2) server. This tool enables remote management and control of systems with the RubyRAT bot installed.

## Disclaimer and Warning

**Warning:** This tool is intended for educational purposes only. Unauthorized use of RubyRAT for malicious activities is strictly prohibited. The developer will not be held responsible for any misuse of the tool.

## Note

This project is under active development, and its stability may vary as it continues to evolve.

# VirusTotal Talk

> ⚠️ **Important:** Please refrain from uploading any payloads to VirusTotal.com, as this could potentially interfere with the functionality of the tool over time. VirusTotal shares signatures with antivirus companies, which may impact the effectiveness of the tool.

### VirusTotal Upload

The tool has already been uploaded once. You can review the upload without any encryption added [here](https://www.virustotal.com/gui/file/426ed7a22f44beac5e34ffc0c71f927749d53e3f8a970acbabbf763894edc1bf/detection).

# Setup Step-By-Step

### 1. Create Discord Bot

1. Go to the [Discord Developer Portal](https://discord.com/developers/applications) and log in to your Discord account.
2. Create a new application and name it. The name will be your bot's display name.
3. Under the "Bot" section, create a bot and copy its token.
4. Customize your bot's appearance, if desired.

### 2. Create Discord Server

1. Create a new Discord server.
2. Inside the new server, add a category called "RubyRAT."

### 3. Integrate BOT into new Server

1. Invite the created bot using an OAuth2 invite link with necessary permissions.
2. Paste the invite link in a browser and follow prompts to add the bot.
3. Configure bot permissions in the "RubyRAT" category.

# Features

### GENERAL
`!help`: Displays a help menu of all commands.
`!cmd <command>`: Execute a shell command.
`!restart`: Restarts client.
`!version`: Prints out current version of RubyRAT.
`!kill`: Disconnects Client from C2.

### FILE MANAGEMENT
`!upload`: Upload a file from C2 to client.
`!download`: Download file from client to C2.
`!cat`: Reads and outputs the content of a file.

### SUVAILLANCE
`!screenshot`: Takes a screenshot on client side and sends to C2.
`!webcam`: Grabs a picture using clients webcam and sends it.
`!info`: Get system info such as GPU, CPU, RAM, HDD/SSD and more.
`!geolocate`: Calculates position using google maps API.

### PERSISTENCE
`!elevate`: Tryies to elevate from User to Admin.
`!install`: Will try to gain persistence on the client system.
`!uninstall`: Will remove persistence from client system.
`!better_install`: Gets presistence on system with 3 diffrent methods.
`!better_uninstall`: Removes 3 diffrent methods of persistence from client system.
`!better_check`: Checks if 3 diffrent methods of persistence is active on client system.
`!cool_install`: Installs persistence with a interesting exploit found in some computers.
`!cool_uninstall`: Uninstalls the exploit persistence found in some computers.

### RECOVERY
`!history`: Gather and download all web history from client.
`!passwords_chrome`: If client has chrome, it will retrieve all saved usernames, passwords and URL's to the website.
`!network`: Retrieve all saved network SSIDs their passwords.

### KEYLOGGER
`!dumplog`: Sends a file containing the keyloggers log to C2.
`!startlog`: Start the keylogging.
`!stoplog`: Stop the keylogging.

### ENCRYPTION
`!encrypt <filename> <password>`: Encrypt a file with a special password.
`!decrypt <filename> <password>`: Decrypt a file with the special password.

### DNS Spoofing
`!block_website <domain>`: Blocks a website on the clients computer.
`!unblock_website <domain>`: Unblocks a website on the clients computer.
`!spoof_dns <domain> <ip>`: Spoofs a domain to a specified IP.

### MISC
`!volume <volume_amount>`: Changes volume to the given procentage
`!sendkey <Hello-World!>`: Sends keystrokes to client, "-" is space
`!messagebox <error\warning\info> <title> <content>`: Sends a custom made messagebox to client.
`!securefile <filepath>`: Nulls out file to make it near-impossible to recover.
`!BSOD <svchost\explorer>`: Diffrent ways to make clients system unstable.
`!interactive_cmd`: Starts a CMD terminal in discord. (Avoid using commands that output much information, like "dir")

# Future Features

1. Gather Browser Cookies
2. Gather Discord Token/s
3. Cross-Platform Compatibility
4. Error Handling and Validation
5. Real-time Microphone Listening
6. Fake Windows Logon
7. Reverse Proxy
8. Save client information automaticly into seperate channel
9. USB Spread

# Warning

Use this tool only for educational purposes. I will not be responsible for any malicious use.
