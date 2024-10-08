﻿# **Memory Game**

## **Overview**
This is a C# console-based **Memory Game**, where players can match pairs of cells on a game board. Players can choose to either play against a friend or against an AI-controlled computer opponent. The game supports different board sizes and comes with an AI that simulates memory, making the gameplay both challenging and engaging.

## **Features**
- **Single-player Mode**: Play against a computer AI that can "remember" previous moves to increase difficulty.
- **Multiplayer Mode**: Play with a friend locally.
- **Customizable Board Size**: The game allows board dimensions ranging from 4x4 to 6x6.
- **AI Memory**: The computer opponent stores up to 5 previous moves and can make decisions based on this memory.
- **Simple Text-based Interface**: Players enter coordinates (rows and columns) to make moves.
- **Replay Option**: Players are given the option to replay after completing a game.

## **Gameplay**
1. **Setup**: The game starts by asking for the player's name and whether they want to play against another player or the computer.
2. **Board Setup**: The player selects the dimensions of the board, which must be an even number of cells.
3. **Turns**: Players take turns revealing two cells on the board. If the cells match, they score a point and continue their turn.
4. **Game End**: The game ends when all pairs have been matched. The player with the highest score wins.

## **How to Run**

### **Prerequisites**
- Install [.NET SDK](https://dotnet.microsoft.com/download) to compile and run the project.
- Any C#-compatible IDE (e.g., Visual Studio, Rider, or Visual Studio Code).

### **Steps to Run the Game**
1. Clone the repository from GitHub:

   ```bash
   git clone https://github.com/keren5005/Memory-Game-in-C-.NET.git

### **Example Board**

Below is an example of how the board might look during gameplay:

![image](https://github.com/user-attachments/assets/c4e0df17-6b98-4011-bb8d-fc9e6caf1e07)

### **Project Structure**

1. Program.cs: The entry point of the game, which initializes the UI and game logic.
2. GameUI.cs: Handles user input, printing the board, and managing game flow.
3. Board.cs: Contains the logic for initializing the game board, revealing cells, and 4/storing their values.
4. Player.cs: Manages both human and AI players. The AI player has memory functionality for advanced gameplay.
5. GameManager.cs: Controls the game logic, such as switching turns, checking for matches, and calculating scores.

### **Sample Game Flow**
![image](https://github.com/user-attachments/assets/eb95351a-1c57-4c1d-b98a-b4307bb11556)

