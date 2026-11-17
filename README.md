# GED In-Class Assignment 2

Name: Christopher Evans 
ID: 100968759

Title: E.M.D (Everyone. Must. Die)
Brief Description + backstory: E.M.D is a first person and top down shooter following a man trying to live a life away from his past, however his past creeps closer to him and the voice in his head craves the rush. With people hunting him and illusions of his own mind can he truly find peace after the work he's done? 

Gameplay loop: The main gameplay of E.M.D has the player enter into a level, from there the player has to find whatever weapon they can (or use their fists) and kill every enemy in the level. Once this is complete the player must walk back to the start of the level, where they will enter their car and finish the level. Death is quick and respawning is easy to get the player right back into the action.

Profiler Use:

Before: <img width="1279" height="766" alt="Screenshot 2026-11-17 130012" src="https://github.com/user-attachments/assets/1a9301e4-3feb-49c5-b3a2-b25e25a8a09c" />

After: <img width="1264" height="757" alt="Screenshot 2026-11-17 143504" src="https://github.com/user-attachments/assets/050f33ae-1220-4139-b61d-22a8eca76b4e" />

Question 1: I noticed that during playtime the player loop seemed to take up a good chunk of space, sometimes this would even be the most memory consuming part of the game during testing. When I looked deeper I saw that the script "BetterEnemy" and its update loop was taking up a decent chunk of that space.

Question 2: I decided to Implement Dirty Flag into my project inside of the BetterEnemy script. I created a variable called comparePOS. This compared the enemy's position with the players current position. I then have it check to see if the player is a certain distance away from the enemy's x,y position (I've come accustomed to unreal so I forgot that Y is up for unity and Z is up for unreal, however it still improved performance nonetheless). If the player and enemy are close enough it'll then allow the enemy to move about in the level, patrolling, hunting and attacking the player.

Question 3: The Dirty Flag does indeed improve the game's performance. The PlayerLoop doesn't take up as much usage as before. Before the PlayerLoop would spike above the EditorLoop and hover around the 40% range. With the Dirty Flag however it now hovers between 20-30%, taking up much less space during runtime.

Question 4: The profiling process revealed that the biggest culprit for taking up data was the PlayerLoop. Inside there I was able to find that the BetterEnemy's Update function was taking up a good chunk of the PlayerLoop. Upon adding the Dirty Flag to the BetterEnemy script to check to see if the player is within a certain range before allowing the enemy to roam/move/attack. After implementation I saw a good decrease in the PlayerLoop showing that my method had worked. However I saw that the EditorLoop was taking up more space now since it had more space to work with without the PlayerLoop taking up as much space as it did.
