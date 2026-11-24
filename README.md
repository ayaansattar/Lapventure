# Lapventure

A mobile idle racing game with a motorsport theme, built in Unity. Your kart laps a circuit and earns money every time it crosses the line. Spend that money upgrading your team, manage tyre wear and pit stops, and climb from the back of the grid to win the championship.

> Portrait-first, built for Android, playable in the Unity Editor with mouse and scroll wheel.

---

## Gameplay

### The core loop
1. **Your kart laps the track** and earns **money per lap**.
2. **Collect** your earnings (or hire a Race Engineer to collect them automatically).
3. **Upgrade departments** to make the kart faster and each lap worth more.
4. **Overtake the rivals**, reach P1 and hold the lead for a full lap to become **Champion**.

### Departments
There are four departments, each with its own `DepartmentConfig` asset in `Assets/Config/`:

| Department | Base cost | Income / level |
|-----------|-----------|----------------|
| Engine    | $1        | +$1.0 / lap    |
| Tires     | $1.5      | +$1.2 / lap    |
| Aero      | $2.2      | +$1.4 / lap    |
| Chassis   | $3        | +$1.6 / lap    |

- Each level costs **7.5% more** than the previous one, up to **level 150**.
- **Star milestones** at levels **25 / 50 / 100 / 150** each **double** that department's income.
- Kart **speed** depends on the share of all department levels bought (4 → 12 units/sec).
- The **Buy x1 / x10 / MAX** toggle sets how many levels one tap buys, for every department.

### Team Upgrades
The Upgrades popup sells one-off upgrades that multiply department income (e.g. *Racing Fuel*: Engine x2, *Energy Drink Sponsor*: all x2, *Title Sponsor*: all x3). Only the cheapest few unbought upgrades are shown, and a badge appears when you can afford one.

### Tyres & pit stops
- Tyres lose **12% per lap**. Below 30% the kart slows to 60% speed.
- Tap **BOX BOX** to call the kart in. It drives into the pit lane on its next pass, slows to the pit-lane speed limit and stops at the pit box.
- A **timing mini-game** starts at the box. Stop the marker in the green zone for a **perfect stop**: a quicker stop plus a **x1.3 speed boost for 10 seconds**.

### Crew
| Hire | Cost | Effect |
|------|------|--------|
| Race Engineer | $75  | Collects lap money automatically. Required for offline earnings. |
| Pit Crew      | $400 | Pits automatically when tyres reach 20%. Full-rate offline earnings. |
| Extra mechanic| $600, x1.8 each | Shorter automatic pit stops (crew of up to 6). |

### Rivals & the championship
- There are four rival karts, from slow to nearly as fast as a fully upgraded kart. Their speeds are set as a share of the player's possible performance.
- Rubber-banding caps how far each rival can pull ahead of you, so you're never hopelessly behind.
- The live **leaderboard** shows position and lap. Tap a kart, or its row on the leaderboard, to highlight it.
- Win by reaching **P1** while at least as fast as every rival, then **hold the lead for one lap**.

### Offline earnings
With a Race Engineer hired, your team keeps earning while the game is closed, for up to **2 hours**. Without a Pit Crew you earn **40%** of the full rate. A "While you were away" panel shows what you earned when you return.

---

## Getting started

### Requirements
- **Unity 6000.6.7f1** (Unity 6)
- Android Build Support module if you want to build for mobile (min SDK 26)

### Running
1. Clone the repo and open the folder in Unity Hub.
2. Open `Assets/Scenes/SampleScene.unity`.
3. Press **Play**. In the editor, drag to pan and use the scroll wheel to zoom. On device, drag and pinch.

### Building
`File → Build Profiles → Android → Build`. Builds go to `Build/` (git-ignored).

### Resetting progress
Progress is saved to `save.json` in `Application.persistentDataPath`. To wipe it, use the **SaveManager** component's ⋮ menu → **Delete Save**, or the in-game *Restart Championship* button.

---

## Project structure

```
Assets/
├── Art/           Cars (karts, junior formula), scenery, track textures, UI icons
├── Config/        DepartmentConfig assets (Engine, Tires, Aero, Chassis)
├── Prefabs/       UpgradeRow (one row in the Upgrades popup)
├── Scenes/        SampleScene: the whole game
├── Scripts/       All gameplay code (below)
└── Settings/      URP / 2D renderer settings
```

### Scripts

**Economy**
| Script | Role |
|--------|------|
| `CurrencyManager` | Singleton holding the player's money. Raises `OnMoneyChanged`. |
| `LapPayout` | Pays `moneyPerLap` on each lap, either banked as uncollected or auto-collected. |
| `Department` / `DepartmentConfig` | Levelled upgrades with exponential cost, stars and income. |
| `TeamStats` | Turns department levels into kart speed and money per lap. |
| `TeamUpgradesManager` / `UpgradeRow` | One-off income multipliers and their popup list. |
| `CrewManager` | Race Engineer, Pit Crew and mechanic hires. |
| `BuyMode` / `BuyModeToggle` | Shared x1 / x10 / MAX purchase setting. |
| `NumberFormat` | Idle-style number formatting (K, M, B, T, Qa, Qi). |

**Racing**
| Script | Role |
|--------|------|
| `KartMover` | Moves a kart along the spline track and counts laps. |
| `RaceManager` | Positions, leaderboard, rival speeds and rubber-banding. |
| `ChampionshipManager` | Win condition and progress bar. |
| `PitStopManager` | Tyre wear, BOX BOX, timing mini-game, boost and auto-pit. |
| `PitLane` | Steers the kart into the pit lane, to the box and back out. |

**Track & visuals**
| Script | Role |
|--------|------|
| `TrackBuilder` | Builds the closed spline from a list of points (⋮ → *Build Track*). |
| `TrackRenderer` | Draws asphalt, kerbs and the start/finish line from the spline. |
| `PitLaneRenderer` | Draws the pit lane and pit box. |
| `SceneryScatter` | Places trees around the circuit in the editor (⋮ → *Scatter Trees*). |
| `CameraFit` / `CameraPanZoom` | Fits the track between the HUD bars, then pinch-zoom and pan. |

**UI**
| Script | Role |
|--------|------|
| `MoneyDisplay`, `IncomeDisplay` | Money and +$/lap readouts. |
| `CollectButton` | Pulsing "Collect $X" button. |
| `TireGauge` | Radial tyre-wear gauge that changes colour from green to yellow to red. |
| `KartInfoPopup`, `KartClickable`, `LeaderboardTap` | Tap a kart to see its position and lap. |
| `SafeArea`, `TopBarBackground` | Notch and safe-area handling. |

**Persistence**
| Script | Role |
|--------|------|
| `SaveManager` / `SaveData` | JSON save, autosave every 30s, save on pause/quit, offline earnings. |

---

## Tuning

Most balance numbers are public fields, so you can tweak them in the Inspector without touching code:
- **Department curves**: edit the assets in `Assets/Config/`.
- **Speed & base income**: `TeamStats` (`startSpeed`, `maxSpeed`, `baseIncome`).
- **Crew prices**: `CrewManager`.
- **Tyres, boost, pit timing**: `PitStopManager`.
- **Rival difficulty**: `RaceManager.rivalPerformance` and `leadCaps`.
- **Offline cap & rate**: `SaveManager` (`maxOfflineHours`, `noCrewOfflineRate`).
- **Team upgrades list**: `TeamUpgradesManager.upgrades`.

> Note: the values in the scene override the script defaults. The numbers in this README are the ones set in `SampleScene`.

---

## Tech
- Unity 6 · Universal Render Pipeline (2D)
- Unity Splines (track path)
- Input System + Enhanced Touch (pan / pinch)
- TextMesh Pro + uGUI
