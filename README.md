# RubyRAT
RubyRAT is a remote access trojan developed in Python, designed to utilize a Discord bot as a Command and Control (C2) server. This tool allows you to remotely manage and control systems that have the RubyRAT bot installed.

## Disclaimer and Warning
**Warning:** This tool is intended for educational purposes only. Unauthorized use of RubyRAT for malicious activities is strictly prohibited. The developer will not be held responsible for any misuse of the tool.

## Note
Please be aware that this project is currently under development, and its stability may vary as it continues to evolve.
⚠️ **Important:** Please refrain from uploading any payloads to VirusTotal.com, as this could potentially interfere with the functionality of the tool over time. VirusTotal shares signatures with antivirus companies, which may impact the effectiveness of the tool.

# Setup Step-By-Step

### 1. Create Discord Bot
   1. Go to the [Discord Developer Portal](https://discord.com/developers/applications) and log in to your Discord account.
   2. Click on the "New Application" button and give your application a name. This name will be the display name for your bot.
   3. In the left sidebar, click on "Bot" and then click on the "Add Bot" button.
   4. Under the "TOKEN" section, click on the "Copy" button to copy your bot's token.
   5. If you want to you can customize your bot's appearance by adding a profile picture, username, and other optional details.

### 2. Create Discord Server
   1. Create a new discord server.
   2. Inside the new server, create a new category called "RubyRAT."
   3. Now you are done with the Server setup.

### 3. Integrate BOT into new Server
   1. Invite the created Discord bot to your new server by generating an OAuth2 invite link with the necessary permissions. To do this, follow these steps:
      - Go to the [Discord Developer Portal](https://discord.com/developers/applications) and select your bot application.
      - Under the "OAuth2" section, scroll down to the "OAuth2 URL Generator" and select the "bot" scope. Also add Administrator to the bot so that it can interact with all needed channels and messages.
      - Copy the generated OAuth2 invite link.
   2. Paste the generated invite link into a web browser and follow the prompts to add the bot to your server:
      - Open a new tab in your web browser and paste the copied OAuth2 link.
      - Select the server you created in the drop-down menu.
      - Click the "Authorize" button to proceed.
   3. Once the bot is added, configure its permissions within the "RubyRAT" category to ensure it can interact with all channels and members as intended:
      - Within your Discord server, locate the "RubyRAT" category that you previously created.
      - Right-click on the category and select "Edit Category."
      - In the "Permissions" section, ensure that your bot has the required permissions to view and send messages, manage channels, and perform other relevant actions within this category.


# Features:

### Basics
      1) !help (Displays a help menu of all commands)
      2) !kill (Disconnects Client from C2)

### Surveillance
      1) !screenshot (Takes a screenshot on client side and sends to C2)
      2) !webcam (Grabs a picture using clients webcam and sends it)

### Elevation / Persistence
      1) !elevate (Tryies to elevate from User to Admin)
      2) !install (Will try to gain persistence on the client system)
      3) !uninstall (Will remove persistence from client system)

### Files
      1) !upload (Upload a file from C2 to client.)
      2) !download <FileName> (Download file from client to C2)
      3) !cat <FileName> (Reads and outputs the content of a file)

### Information
      1) !info (Get system info such as GPU, CPU, RAM, HDD/SSD and more.)


# Future Features
    1) Gather Browser History
    2) Gather Browser Passwords
    3) Gather System Information
    4) Gather Browser Cookies
    5) Gather Discord Token/s
    6) Data Encryption/Decryption
    7) Cross-Platform Compatibility
    8) Error Handling and Validation
    

# Warning:-
    Use this tool Only for Educational Purpose And I will Not be Responsible For ur cruel act.
