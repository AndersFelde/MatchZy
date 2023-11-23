## Chat Commands

- .ban [mapname]
- .pick [mapname]
- .stay
- .switch
- .ready
- .unready
- .pause
- .unready
- .help

## Console commands

- get5 - Gets version
- get5_start [match_name] - starts a match
- get5_stop - stops a match

## Json format

`match_name.json`

```json
{
  "teamName1": "TeamA", //must match team file
  "teamName2": "TeamB", //must match team file
  "matchTitle": "TeamA vs TeamB", //default TeamA vs TeamB
  "numMaps": 1, // default: 1
  "minPlayersToReady": 5, //default: 5
  "voteFirst": "random", // ["team1", "team2", "random"]
  "mapSides": "knife", // ["team1_ct", "team2_ct", "knife"]
  "voteMode": "ban", // ["ban", "pick"]
  "mapList": ["de_dust2", "de_mirage", "de_inferno"]
}
```

`TeamA.json`

```json
{
  "teamName": "TeamA", //defaults to filename
  "teamFlag": "NO", //defaults to NO
  "teamTag": "TA", //defaults to first two chars in team name
  "players": [
    {
      "name": "user1",
      "steamID": 12345678912345678 //17 characters long
    },
    {
      "name": "user2",
      "steamID": 12333333912345678
    }
  ]
}
```
