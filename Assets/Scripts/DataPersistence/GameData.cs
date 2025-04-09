[System.Serializable]
public class GameData {
    
    public int HighestWaveSurvived;
    public float TimeSpent;
    
    
    
    public GameData() {
        set(0, 0);
    }
    
    public GameData(GameData d) {
        set(d.HighestWaveSurvived, d.TimeSpent);
    }
    
    void set(int hws, float ts) {
        HighestWaveSurvived = hws;
        TimeSpent = ts;
    }
    
}