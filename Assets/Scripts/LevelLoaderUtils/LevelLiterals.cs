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

}