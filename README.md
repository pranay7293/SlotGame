# 🎰 Real-Time Unity Slot Machine (Unity 2D)

A polished 3-reel slot machine demo built in Unity, featuring:

- **9 distinct symbols** (Seven, Bar, Diamond, Bell, Cherry, Lemon, Watermelon, Grapes, Orange)  
- **94.33% RTP** via custom symbol distributions  
- **Smooth spin & cubic deceleration** powered by coroutines  
- **Account & betting system** (starts at 100, 1-unit bet per spin)  
- **Single center payline** with both 3-of-a-kind and 2-of-a-kind payouts  
- **Interactive paytable UI** and **on-demand RTP simulation** with live 95% confidence interval

---

## 🧠 Game Concept

- **Three vertical reels**, each with 30 stops and no blanks  
- One **center row payline**—only those three symbols matter  
- **3-of-a-kind** pays: Seven ➔ 1000×, Bar ➔ 500×, Diamond ➔ 250×, Bell ➔ 150×, Cherry ➔ 80×  
- **2-of-a-kind** pays: Seven ➔ 100×, Bar ➔ 75×, Diamond ➔ 40×, Bell ➔ 30×, Cherry ➔ 25×  
- Other symbols (Lemon, Watermelon, Grapes, Orange) do **not** pay  
- **AccountManager** tracks balance, enforces bets, credits wins  
- **SimulationManager** runs up to millions of spins off-screen, displays RTP ±95% CI  

---

## ▶️ How to Play

1. **Launch** the game (WebGL in browser or install the APK).  
2. Your **balance** starts at **100**. Each spin costs **1**.  
3. Click **SPIN** (or press Spacebar) to spin all three reels.  
4. If the **center symbols** match, you win according to the paytable.  
5. Watch your balance update; play until you cash out or run out of funds.  
6. In the **Simulation** panel, enter any spin count, and click **Run Custom** to verify RTP in-game.

---

## ▶️ Try It Now

[![Play on WebGL](https://img.shields.io/badge/Play-WebGL-green?style=for-the-badge)](https://example.com/slot-machine/player)  
> *Runs in browser. No download needed.*

---

## 📦 Download APK

[![Download APK](https://img.shields.io/badge/Download-APK-blue?style=for-the-badge)](https://github.com/your-username/YourRepo/releases/download/v1.0/SlotMachine.apk)  
> *Install on Android devices. Enable “Unknown Sources.”*

---

## 💰 Paytable

| Symbol       | 3-of-a-kind × | 2-of-a-kind × |
|:-------------|--------------:|-------------:|
| Seven        |        1000×  |       100×   |
| Bar          |         500×  |        75×   |
| Diamond      |         250×  |        40×   |
| Bell         |         150×  |        30×   |
| Cherry       |          80×  |        25×   |
| Lemon        |           0×  |         0×   |
| Watermelon   |           0×  |         0×   |
| Grapes       |           0×  |         0×   |
| Orange       |           0×  |         0×   |

---

## 🎁 Key Features

- ✅ **Precision RTP** (94.33%) via symbol-frequency math  
- ✅ **Seamless reel wrap** (double-strip content)  
- ✅ **Variable spin duration** (extra loops + deceleration)  
- ✅ **Account & UI**—balance, bet enforcement, win display  
- ✅ **Paytable UI**—dynamic list of winning combos  
- ✅ **On-demand Simulation**—enter spin count, view RTP ±95% CI  
- ✅ **Responsive Canvas**—adapts to multiple resolutions  
- ✅ **No external tween plugins**—pure coroutine-based motion

---

## 🛠️ Project Setup

### Requirements
- **Unity 2021.3 LTS** (or newer)  
- **TextMeshPro** package  
- **Canvas Scaler** configured for Screen-Space Overlay  

### Clone & Open

```bash
git clone https://github.com/your-username/YourRepo.git
cd YourRepo
