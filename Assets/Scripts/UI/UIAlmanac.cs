using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIAlmanac : UIPanel {
    
    public enum EAlmanacCategory {
        Player,
        Enemies
    }
    
    struct AlmanacDataEntry {
        public readonly string name;
        public readonly string desc;
        public readonly string imageURL;
        
        public AlmanacDataEntry(string name, string desc, string imageURL) {
            this.name = name;
            this.desc = desc;
            this.imageURL = imageURL;
        }
    }
    
    readonly AlmanacDataEntry[] enemyDataEntries = new AlmanacDataEntry[] {
        new(
            name: "Bug",
            desc: "This be what the bug do. It do, what it be of can, where when it shall.",
            imageURL: "Resources/Almanac/bug_thumbnail"
        ),
        new(
            name: "Hunter",
            desc: "Lorem ipsum type beat.",
            imageURL: "Resources/Almanac/hunterBasic_thumbnail"
        ),
        new(
            name: "Empowered Hunter",
            desc: "Lorem ipsum type beat.",
            imageURL: "Resources/Almanac/hunterEmpowered_thumbnail"
        ),
        new(
            name: "Crab",
            desc: "Lorem ipsum type beat.",
            imageURL: "Resources/Almanac/crabBasic_thumbnail"
        ),
        new(
            name: "Laser Crab",
            desc: "Lorem ipsum type beat.",
            imageURL: "Resources/Almanac/crabLaser_thumbnail"
        ),
        new(
            name: "Turtle",
            desc: "Lorem ipsum type beat.",
            imageURL: "Resources/Almanac/turtle_thumbnail"
        ),
        new(
            name: "Centipede",
            desc: "Lorem ipsum type beat.",
            imageURL: "Resources/Almanac/centipede_thumbnail"
        ),
    };
    
    readonly AlmanacDataEntry[] playerDataEntries = new AlmanacDataEntry[] {
        new(
            name: "Player Entry Title",
            desc: "Description here.",
            imageURL: "Resources/Almanac/THUMBNAIL_IMAGE"
        ),
    };
    
    [Header("Tab Button References")]
    public Button EnemyTabButton;
    public Button PlayerTabButton;
    
    [Header("Main Display UI References")]
    [SerializeField]
    TMP_Text EntryNameLabel;
    [SerializeField]
    TMP_Text EntryDescriptionLabel;
    [SerializeField]
    RawImage EntryImage;
    
    [Header("Side Button References")]
    [SerializeField]
    GameObject[] SidebarEntryButtons;
    TMP_Text[] sidebarEntryButtonLabels;
    
    [HideInInspector]
    public EAlmanacCategory CurrentAlmanacCategory = EAlmanacCategory.Enemies;
    
    Texture2D currentLoadedImage;
    
    
    
    public override void Init() {
        base.Init();
#if UNITY_EDITOR
        // Assert that the number of sidebar buttons is valid
        int longestLength = Mathf.Max(enemyDataEntries.Length, playerDataEntries.Length);
        int numButns = sidebarEntryButtonLabels.Length;
        if (numButns < longestLength)
            throw new System.Exception($"ERROR: Make sure the number of Almanac sidebar buttons ({numButns}) is equal to the length of the longest data entry array ({longestLength}).");
        else if (numButns > longestLength)
            Debug.LogWarning($"WARN: There are more Almanac sidebar buttons ({numButns}) than the length of the longest data entry array ({longestLength}).");
#endif
        sidebarEntryButtonLabels = new TMP_Text[SidebarEntryButtons.Length];
        for (int i = 0; i < SidebarEntryButtons.Length; i++) {
            sidebarEntryButtonLabels[i] = SidebarEntryButtons[i].GetComponentInChildren<TMP_Text>();
        }
        updateSidebuttons();
    }
    
    public void OnButton_ChangeTabTo(EAlmanacCategory categoryTab) {
        CurrentAlmanacCategory = categoryTab;
        updateSidebuttons();
        viewEntry(0);
    }
    
    public void OnButton_ViewEntry(int index) {
        viewEntry(index);
    }
    
    void viewEntry(int index) {
        AlmanacDataEntry entry = CurrentAlmanacCategory switch {
            EAlmanacCategory.Enemies => enemyDataEntries[index],
            EAlmanacCategory.Player => playerDataEntries[index],
            _ => throw new System.Exception($"The Almanac category value of {(int)CurrentAlmanacCategory} is an impossible value.")
        };
        EntryNameLabel.text = entry.name;
        EntryDescriptionLabel.text = entry.desc;
        loadAndSetEntryImageAsync(entry.imageURL);
    }
    
    IEnumerator loadAndSetEntryImageAsync(string path) {
        ResourceRequest rr = Resources.LoadAsync<Texture2D>(path);
        yield return rr;
        Resources.UnloadAsset(currentLoadedImage);
        currentLoadedImage = rr.asset as Texture2D;
        EntryImage.texture = currentLoadedImage;
    }
    
    void updateSidebuttons() {
        int highestWave = DataPersistenceManager.Instance.GetGameData().HighestWaveCompleted;
        AlmanacDataEntry[] entries = CurrentAlmanacCategory switch {
            EAlmanacCategory.Enemies => enemyDataEntries,
            EAlmanacCategory.Player => playerDataEntries,
            _ => throw new System.Exception($"The Almanac category value of {(int)CurrentAlmanacCategory} is an impossible value.")
        };
        for (int i = 0; i < SidebarEntryButtons.Length; i++) {
            if (i < entries.Length) {
                SidebarEntryButtons[i].SetActive(true);
                if (CurrentAlmanacCategory == EAlmanacCategory.Enemies) {
                    sidebarEntryButtonLabels[i].text = hasDiscoveredEnemy(i, highestWave) ? entries[i].name : "???";
                } else {
                    sidebarEntryButtonLabels[i].text = entries[i].name;
                }
            } else {
                SidebarEntryButtons[i].SetActive(false);
            }
        }
    }
    
    bool hasDiscoveredEnemy(int enIndex, int highestWave) {
        // NOTE: HARDCODED VALUES. Make sure to update when the wave table file is updated. Consider based on the wave right before an enemy spawns
        return enIndex switch {
            // Bug
            0 => true,
            // Hunter basic
            1 => highestWave >= 1,
            // Hunter empowered
            2 => highestWave >= 14,
            // Crab basic
            3 => highestWave >= 3,
            // Crab empowered
            4 => highestWave >= 11,
            // Turtle
            5 => highestWave >= 7,
            // Centipede
            6 => false,
            _ => false,
        };
    }
    
}