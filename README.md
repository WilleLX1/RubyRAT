# RubyRAT
A remote access trojan in python that uses discord bot as a C2

# Warning:-
    Use this tool Only for Educational Purpose And I will Not be Responsible For ur cruel act.
    
# Note:
      This project may not be fully stable as it is a work in progress.


# Setup Step-By-Step

### 1. Create Discord Bot
   1. Go to the [Discord Developer Portal](https://discord.com/developers/applications) and log in to your Discord account.
   2. Click on the "New Application" button and give your application a name. This name will be the display name for your bot.
   3. In the left sidebar, click on "Bot" and then click on the "Add Bot" button.
   4. Under the "TOKEN" section, click on the "Copy" button to copy your bot's token. Keep this token secure and do not share it with anyone.
   5. Customize your bot's appearance and behavior by adding a profile picture, username, and other optional details.

### 2. Create Discord Server
   1. Create a new discord server.
   2. Inside the new server, create a new category called "RubyRAT."
   3. Now you are done with the Server setup.

### 3. Integrate BOT into new Server
   1. Invite the created Discord bot to your new server by generating an OAuth2 invite link with the necessary permissions. To do this, follow these steps:
      - Go to the [Discord Developer Portal](https://discord.com/developers/applications) and select your bot application.
      - Under the "OAuth2" section, scroll down to the "OAuth2 URL Generator" and select the "bot" scope.
      - Below the scope options, select the permissions your bot requires. For example, you might need "Read Messages," "Send Messages," and other relevant permissions.
      - Copy the generated OAuth2 invite link.
   2. Paste the generated invite link into a web browser and follow the prompts to add the bot to your server:
      - Open a new tab in your web browser and paste the copied OAuth2 link.
      - Select the appropriate server from the drop-down menu. Make sure you have the necessary permissions to add the bot to the server.
      - Click the "Authorize" button to proceed.
   3. Once the bot is added, configure its permissions within the "RubyRAT" category to ensure it can interact with the channels and members as intended:
      - Within your Discord server, locate the "RubyRAT" category that you previously created.
      - Right-click on the category and select "Edit Category."
      - In the "Permissions" section, ensure that your bot has the required permissions to view and send messages, manage channels, and perform other relevant actions within this category.
      - Adjust the bot's role or permissions as needed to match the functionality you want it to have.


# Features:

### Basics
      1) !kill (Disconnects Client from C2)

### Surveillance
      1) !screenshot (Takes a screenshot on client side and sends to C2)
      2) !webcam (Grabs a picture using clients webcam and sends it)

### Files
      1) !upload (Upload a file from C2 to client.)
      2) !download <FileName> (Download file from client to C2)

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
