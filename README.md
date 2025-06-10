# VRBuckets – A Simple Multiplayer VR Basketball Game

## What This Is

This project was my first time building a multiplayer VR game using Normcore. The goal was to create a simple basketball game where players could join together in VR and compete to make the most shots. It started off pretty straightforward, but once I got into syncing gameplay between multiple players, things got a lot more complex.

## There are two game modes:

    Free Play: one shared ball, no time limit, just shoot for fun

    Time Attack: 90 seconds to score as many points as possible with unlimited balls

I tried to keep the core gameplay tight and responsive while making sure everything synced correctly across the network.

## Challenges and Learnings

The hardest parts of the project mostly had to do with multiplayer syncing:

#### Game State Syncing:
  Making sure both players always saw the correct score, combo multiplier, and winner was more involved than I expected. Small desyncs caused bigger problems if not handled carefully.
#### Resetting the Game:
  Since players don’t own all the same objects, one person couldn’t just reset everything for everyone. I had to create a way to send a signal that told both players to reset their own local game state.
#### Throwing Physics: 
  Getting throwing to feel right was trickier than expected. I had to find a balance where both soft, gentle tosses and fast, long throws felt good in VR. This took a lot of iteration and tuning.

## Final Thoughts

Even though it was challenging, I really enjoyed this project. I learned a lot about how multiplayer VR works and how to think about networking, ownership, and physics all at the same time. There's still more I’d love to add, but I’m proud of how it turned out within the time I had.
