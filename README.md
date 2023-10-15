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

### Basics
1. `!help`: Displays a help menu of all commands.
2. `!kill`: Disconnects client from C2.
3. `!restart`: Restarts client.
4. `!version`: Prints out current version of RubyRAT.

### Surveillance
1. `!screenshot`: Takes a screenshot on the client side and sends it to C2.
2. `!webcam`: Captures a picture using the client's webcam and sends it.
3. `!dumplog`: Sends a file containing the keyloggers log to C2.
4. `!startlog`: Start the keylogging.
5. `!stoplog`: Stop the keylogging.
6. `!geolocate`: Calculates position using google maps API.
        
### Elevation / Persistence
1. `!elevate`: Attempts to elevate from user to admin.
2. `!install`: Tries to gain persistence on the client system.
3. `!uninstall`: Removes persistence from the client system.

### Files
1. `!upload`: Uploads a file from C2 to the client.
2. `!download <FileName>`: Downloads a file from the client to C2.
3. `!cat <FileName>`: Reads and outputs the content of a file.
4. `!encrypt <filename> <password>`: Encrypt a file with a special password.
5. `!decrypt <filename> <password>`: Decrypt a file with the special password.

### Information
1. `!info`: Retrieves system info including GPU, CPU, RAM, HDD/SSD, and more.
2. `!history`: Gather and download all web history from client.
3. `!network`: Retrieve all saved network names their passwords

### Fun
1. `!volume <volume_amount>`: Changes volume to the given procentage.
2. `!sendkey <Hello-World!>`: Sends keystrokes to client ("-" is space).
3. `!messagebox <error\warning\info> <title> <content>`: Sends a custom made messagebox to client.

# Future Features

1. Gather Browser Passwords
2. Gather Browser Cookies
3. Gather Discord Token/s
4. Cross-Platform Compatibility
5. Error Handling and Validation
6. Real-time Microphone Listening

# Warning

Use this tool only for educational purposes. I will not be responsible for any malicious use.

