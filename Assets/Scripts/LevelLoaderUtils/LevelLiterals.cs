public static class LevelLiterals
{
    public static string cloneTestLevel = 
        @"
wall,wall,wall,wall,wall,wall,wall
wall,""cloneSpawn:c=red:cloneId=1:eg=200,
    cloneEnd:c=blue:cloneId=0,
    goal:eg=100:c=blue"",,,,""cloneSpawn:c=green:cloneId=2:eg=300,
    cloneEnd:c=red:cloneId=1,
    goal:eg=200:c=red"",wall
    wall,,,,,,wall
        wall,,,,,,wall
        wall,,,boulderGoal,,,wall
        wall,,,boulder,,,wall
        wall,,,wall,,,wall
        wall,,,,,,wall
        wall,,,,,,wall
        wall,""cloneSpawn:c=blue:cloneId=0:eg=100,
    goal:eg=400:c=yellow"",,,,""cloneSpawn:c=yellow:cloneId=3:eg=400,
    cloneEnd:c=green:cloneId=2,
    goal:eg=300:c=green"",wall
    wall,wall,wall,wall,wall,wall,wall
";

    public static string myLevel1 =
        @"wall,wall,wall,wall,wall,wall,wall
wall,cloneSpawn:cloneId=1,,,,goal,wall
wall,,,,,goal,wall
wall,player,,,,,wall
wall,wall,wall,,wall,wall,wall
wall,,,,,,wall
wall,,,,,,wall
wall,,,,,cloneEnd:cloneId=0,wall
wall,wall,wall,wall,wall,wall,wall
";

    public static string myLevel2 = 
        @"wall,wall,wall,wall,wall,wall,wall
wall,cloneSpawn:cloneId=1,,,,goal,wall
wall,,boulder,basicButton:eg=1,,,wall
wall,player,,,,,wall
wall,,,,,,wall
wall,,,,,,wall
wall,wall,wall,basicGate:eg=1,wall,wall,wall
wall,wall,wall,basicGate:eg=1,wall,wall,wall
wall,,,,,,wall
wall,cloneEnd:cloneId=0,,,goal,boulderGoal,wall
wall,wall,wall,wall,wall,wall,wall
";

    public static string sokobanBoulderLevel = 
        @",wall,wall,wall,wall,wall,
,wall,,""player, boulderGoal"",,wall,
    ,wall,boulder,boulderGoal,boulder,wall,
    ,wall,,""boulder, boulderGoal"",,wall,
    ,wall,,""boulder, boulderGoal"",,wall,
    ,wall,,""boulder, boulderGoal"",,wall,wall
        wall,wall,,""boulder, boulderGoal"",,,wall
        wall,,,""boulder, boulderGoal"",,,wall
        wall,,,,,wall,wall
    ,wall,,,wall,wall,
    ,wall,wall,wall,wall,,
";

    public static string pokemonLevel = 
            @"wall,wall,wall,wall,wall,wall,wall
wall,wall,,goal:eg=1,,wall,wall
wall,boulder,boulder,,boulder,boulder,wall
wall,,boulder,boulder,boulder,,wall
wall,boulder,,,,boulder,wall
wall,,boulder,boulder,boulder,,wall
wall,,,,,,wall
wall,,,,,,wall
wall,,,,,,wall
wall,,,cloneSpawn:eg=1:cloneId=0,,,wall
wall,wall,wall,wall,wall,wall,wall
";

    public static string doublePlayerSwitchLevel =
        @"wall,wall,wall,wall,wall,wall,wall
wall,,,,,,wall
wall,,,,,,wall
wall,,wall,,,,wall
wall,,,,,,wall
wall,,cloneSpawn:eg=2:c=blue:spawnOrder=0,goal:eg=1:c=red,,,wall
wall,,cloneSpawn:eg=1:c=red:spawnOrder=0,goal:eg=2:c=blue,,,wall
wall,,,,,,wall
wall,,,,,boulder:c=blue,wall
wall,,,,,,wall
wall,wall,wall,wall,wall,wall,wall
";

    public static string boulderGateLevel =
        @"wall,wall,wall,wall,wall,wall,wall,wall,wall,wall,wall,wall,wall,wall,wall
wall,cloneSpawn:spawnOrder=0:eg=100,wall,,,,,wall,,,,,,,wall
wall,,wall,,boulder,,,wall,,basicButton:eg=3:c=yellow,,,,,wall
wall,,wall,,,,,wall,,,,,,,wall
wall,,wall,,,,,wall,,,basicGate:eg=3:c=yellow,basicGate:eg=3:c=yellow,basicGate:eg=3:c=yellow,,wall
wall,boulder,basicGate:eg=1:c=red,,,basicButton:eg=2:c=blue,,basicGate:eg=2:c=blue,,,basicGate:eg=3:c=yellow,""boulder,  goal:eg=100"",basicGate:eg=3:c=yellow,,wall
    wall,,wall,,,,,wall,,,basicGate:eg=3:c=yellow,basicGate:eg=3:c=yellow,basicGate:eg=3:c=yellow,,wall
        wall,,wall,,,,,wall,,,,,,,wall
        wall,,wall,,,,,wall,,,,,,,wall
        wall,,wall,,,,,wall,,,,,,,wall
        wall,basicButton:eg=1:c=red,wall,,,,,wall,,,,,,,wall
        wall,wall,wall,wall,wall,wall,wall,wall,wall,wall,wall,wall,wall,wall,wall
            ";
    
    public static string aBitOfEverything = 
        @"wall,wall,wall,wall,wall,wall,wall
wall,cloneSpawn:c=blue:cloneId=0:eg=2,,,,,wall
wall,,,,basicButton:eg=4,reverseGate:eg=4,wall
wall,,wall,,,basicGate:eg=4,wall
wall,,,,,,wall
wall,,cloneSpawn:spawnOrder=0:c=red,,,,wall
wall,,,goal:eg=2:c=blue,,,wall
wall,,,,,,wall
wall,,,,,boulder:c=blue,wall
wall,,,,,,wall
wall,wall,wall,wall,wall,wall,wall
";
}