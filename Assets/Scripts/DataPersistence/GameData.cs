[System.Serializable]
public class GameData {
    
    public int HighestWaveCompleted;
    public float[] TimeSpentEachWave;
    
    
    
    public GameData() {
        HighestWaveCompleted = 0;
        TimeSpentEachWave = new float[HighestWaveCompleted];
    }
    
    public GameData(GameData d) {
        HighestWaveCompleted = d.HighestWaveCompleted;
        TimeSpentEachWave = new float[d.TimeSpentEachWave.Length];
        for (int i = 0; i < d.TimeSpentEachWave.Length; i++)
            TimeSpentEachWave[i] = d.TimeSpentEachWave[i];
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