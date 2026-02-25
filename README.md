# osu!RefHelper

A command-line tool designed to assist osu! tournament referees in managing match lobbies, tracking picks/bans, and generating multiplayer lobby commands.

## Features

- **Automated Lobby Setup**: Generates osu! multiplayer lobby commands for tournament matches
- **Pick/Ban Phase Management**: Tracks player bans and map picks throughout the match
- **Score Tracking**: Monitors match progress and determines winners
- **Map Pool Integration**: Reads from `pool.txt` file to manage available maps
- **Alternating Pick Order**: Automatically tracks which player picks next based on roll winner
- **Tiebreaker Support**: Automatically enables tiebreaker when match reaches final point
- **Mod Management**: Automatically sets appropriate mods (NF, HD, HR, DT) based on map slot

## Requirements

- .NET 10 or higher
- Windows, macOS, or Linux

## Setup

1. Clone the repository:

   git clone https://github.com/selmeczi-attila/osu-RefHelper.git
   cd osu-RefHelper


2. Create a `pool.txt` file in the same directory as the executable with your map pool in the following format:

   NM1:beatmap_id
   
   NM2:beatmap_id
   
   HD1:beatmap_id
   
   HR1:beatmap_id
   
   DT1:beatmap_id
   
   TB:beatmap_id


3. Build and run:

   dotnet build
   dotnet run


## Usage

1. **Tournament Information**: Enter the tournament acronym and round (e.g., "HOL Group Stage")

2. **Player Setup**: 
   - Enter Player 1 name (higher seed)
   - Enter Player 2 name
   - Specify the Best of X format (must be odd number)
   - Optionally enter group stage lobby letter

3. **Roll Results**: 
   - Specify who won the roll
   - Choose whether the roll winner gets first pick or first ban

4. **Bans Phase**: Enter the map slots to ban (e.g., "HD1", "DT2")

5. **Match Progress**: 
   - Enter map slots to pick
   - The tool will generate the `!mp map` and `!mp mods` commands
   - After each map, enter the round winner
   - The tool tracks scores and alternating picks automatically

6. **Exit**: Type `q` when prompted for a map pick to exit

## pool.txt Format

Each line should contain a map slot identifier followed by a colon and the beatmap ID:


NM1:123456
NM2:234567
HD1:345678
HR1:456789
DT1:567890
TB1:678901


**Slot naming conventions:**
- `NM` - No Mod
- `HD` - Hidden
- `HR` - Hard Rock
- `DT` - Double Time
- `TB` - Tiebreaker

## Generated Commands

The tool automatically generates the following osu! IRC commands:

- `!mp make` - Creates the multiplayer lobby with correct name
- `!mp set 0 3` - Sets lobby to Score V2, Head to Head
- `!mp invite` - Invites both players
- `!mp map` - Changes to the picked map
- `!mp mods` - Sets appropriate mods (always includes NF)

## Roadmap

- [ ] Add configuration file support for modularity
- [ ] Improve input validation
- [ ] Prevent duplicate player names
- [ ] Code refactoring for better maintainability
- [ ] Enhance pool.txt correctness validation

## Contributing

Contributions are welcome! Feel free to open issues or submit pull requests.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE.txt) file for details.
