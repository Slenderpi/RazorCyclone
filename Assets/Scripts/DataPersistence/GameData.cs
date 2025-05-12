[System.Serializable]
public class GameData {
    
    public int HighestWaveCompleted;
    // public float[] TimeSpentEachWave;
    public float GameTimeSpent;
    
    
    
    public GameData() {
        HighestWaveCompleted = 0;
        GameTimeSpent = 0;
        // TimeSpentEachWave = new float[HighestWaveCompleted];
    }
    
    public GameData(int highestWaveCompleted, float gameTimeSpent) {
        HighestWaveCompleted = highestWaveCompleted;
        GameTimeSpent = gameTimeSpent;
    }
    
    public GameData(GameData d) {
        HighestWaveCompleted = d.HighestWaveCompleted;
        GameTimeSpent = d.GameTimeSpent;
        // TimeSpentEachWave = new float[d.TimeSpentEachWave.Length];
        // for (int i = 0; i < d.TimeSpentEachWave.Length; i++)
        //     TimeSpentEachWave[i] = d.TimeSpentEachWave[i];
    }
    
    public static bool operator true(GameData gd) {
        return gd != null;
    }
    
    public static bool operator false(GameData gd) {
        return gd == null;
    }
    
    public static bool operator !(GameData gd) {
        return gd == null;
    }
    
}