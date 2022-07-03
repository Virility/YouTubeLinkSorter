using System.Text;
using System.Web;

namespace YouTubeLinkSorter
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        public class TopicSorter
        {
            public string MainTopic = string.Empty;
            public Dictionary<string, StringBuilder> TopicWordAssociationStringBuilders;

            public TopicSorter(string mainTopic)
            {
                MainTopic = mainTopic;
                TopicWordAssociationStringBuilders = new Dictionary<string, StringBuilder>
                {
                    { mainTopic, new StringBuilder() }
                };
            }

            public TopicSorter(string[] topicWordAssociations)
            {
                TopicWordAssociationStringBuilders = new Dictionary<string,StringBuilder>();
                foreach (var topicWordAssociation in topicWordAssociations)
                    TopicWordAssociationStringBuilders.Add(topicWordAssociation, new StringBuilder());
            }   
            
            public TopicSorter(string mainTopic, string[] topicWordAssociations)
            {
                MainTopic = mainTopic;
                TopicWordAssociationStringBuilders = new Dictionary<string, StringBuilder>();
                foreach (var topicWordAssociation in topicWordAssociations)
                    TopicWordAssociationStringBuilders.Add(topicWordAssociation, new StringBuilder());
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(richTextBox1.Text)) return;

            var topicSorterPairs = new Dictionary<string, TopicSorter>
            {
                { "assorted before", new TopicSorter(new[] { "famous amos", "company", "plumb" }) },
                { "Depp", new TopicSorter(new[] { "Amber Heard", "depp", "johnny" }) },
                { "Politics", new TopicSorter(new[] { "politic", "democra", "republic", "ammendment", "law", "government", "policy" }) },
                { "Audit", new TopicSorter(new[] { "cop", "officer", "audit", "journalist", "arrest", "right", "security" , "police", "detain", "gun", "sherrif", "court", "pd" }) },
                { "Food", new TopicSorter(new[] { "hobart", "goat", "bread", "tortilla", "steak", "pizza", "taco", "burrito", "kitchen", "But Cheaper", "But Better", "gluten", "beef", "chicken", "Fryer", "Oven", "powder", "curry", "sauce", "salt"}) },
                { "Guitar", new TopicSorter(new[] { "caged", "strat", "les paul", "guitar", "epiphone", "gibson", "fender", "chord", "note", "jimi", "Pickup", "vibrato", "bass", "Fretboard", "fret"}) },
                { "Prod", new TopicSorter(new[] { "Studio", "synth", "ableton", "headphone", "IEM", "sennheiser", "amp", "massdrop", "audio", "mix" , "Loud" , "Plugin", "808", "Instrument", "song", "music", "davis", "ninja", "track" }) },
                { "Electronics", new TopicSorter(new[] { "Solar Panel", "EEVblog", "ElectroBOOM", "diode", "electricity", "repair", "capacit", "resist", "transis", "induct", "multi-meter", "Kaiweet", "multimeter", "circuit", "computer", "tube", "Solder", "current", "voltage", "CPU", "clean", "fan", "motor", "newegg"}) },
                { "Tech", new TopicSorter(new[] { "phone", "linux", "c#", "Svelte", "asp.net", "razor", "repair", "neural", "Deep Learn", "induction", "android app", "app", "scien", "tesla", "internet", "USB-C", "USBC", "energy" }) },
                { "Art", new TopicSorter(new[] { "vfx", "3d", "paint", "draw", "animat" }) },
                { "Nature", new TopicSorter(new[] { "life", "nature", "higgs", "quantum", "physics", "atom", "gravity", "neutr", "water", "univers", "multiverse", "multi-verse", "earth", "Entanglement", "Black Hole", "Wormhole", "Dark Matter", "matter", "mass", "Galax", "Cosmic", "vaccine", "cell", "animal", "plant", "milky", "Big Bang"  }) },
                { "Ed", new TopicSorter(new[] { "bassmaster", "scam", "skippy", "mumble", "Look At", "prank", "burr", "nimesh", "Carbonaro", "carlin" }) },
                { "Car", new TopicSorter(new[] { "car", "vehicle", "aux", "ssd", "batter", "ford" }) },
                { "Materials", new TopicSorter(new[] { "wood", "glass", "metal"}) },
                { "assorted after", new TopicSorter(new[] { "mod", "garbage disposal", "philos", "Newton", "einstein", "muscle" }) },
            };
            foreach (var topicWordPair in topicSorterPairs)
                topicWordPair.Value.MainTopic = topicWordPair.Key;

            var shortsSb = new StringBuilder();

            var mainSb = new StringBuilder();
            foreach (var line in richTextBox1.Lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    mainSb.AppendLine();
                    continue;
                }
                if (!line.Contains("youtube.com")) continue;
               
                var delimeter = " - ";
                var lineSplit = line.Split(delimeter);
                if (lineSplit.Length <= 1) continue;

                var link = lineSplit[lineSplit.Length - 1];
                if (!Uri.IsWellFormedUriString(link, UriKind.Absolute)) continue;

                var title = line.Substring(0, line.Length - link.Length - delimeter.Length);
                if (link.Contains("/watch"))
                {
                    var uri = new Uri(link);
                    var videoId = HttpUtility.ParseQueryString(uri.Query).Get("v");
                    link = "https://www.youtube.com/watch?v=" + videoId;

                    var content = title + delimeter + link;
                    var wasTopicWord = false;
                    foreach (var topicWordPair in topicSorterPairs)
                    {
                        var matchTopic = topicWordPair.Value.TopicWordAssociationStringBuilders.Keys
                            .FirstOrDefault(wordAssociaton => title.Contains(wordAssociaton, StringComparison.OrdinalIgnoreCase));
                        if (matchTopic == null) continue;
                        wasTopicWord = true;

                        var topicSorter = topicWordPair.Value;
                        if (topicSorter.TopicWordAssociationStringBuilders[matchTopic].Length == 0 ||
                            !topicSorter.TopicWordAssociationStringBuilders[matchTopic].ToString().Contains(title))
                            topicSorter.TopicWordAssociationStringBuilders[matchTopic].AppendIndented(content);
                        break;
                    }
                    if (!wasTopicWord && !mainSb.ToString().Contains(title))
                        mainSb.AppendLine(content);
                } else if (link.Contains("/shorts")) {
                    if (!shortsSb.ToString().Contains(title))
                        shortsSb.AppendLine(title + delimeter + link);
                }
            }
            foreach (var topicSorterPair in topicSorterPairs)
            {
                var mainTopicSb = new StringBuilder();
                mainTopicSb.AppendLine(Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(topicSorterPair.Key) + ":");

                foreach (var wordAssociationStringBuilderPair in topicSorterPair.Value.TopicWordAssociationStringBuilders)
                {
                    if (wordAssociationStringBuilderPair.Value.Length <= 0) continue;
                    if (topicSorterPair.Key != wordAssociationStringBuilderPair.Key)
                        mainTopicSb.AppendIndented(Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(wordAssociationStringBuilderPair.Key) + ":");
                    mainTopicSb.AppendIndented(wordAssociationStringBuilderPair.Value.ToString());
                }
                mainSb.Append(mainTopicSb.ToString());
            }

            if (mainSb.Length != 0)
            {
                mainSb.AppendLine("Shorts:");
                mainSb.AppendIndented(shortsSb.ToString() + "\r\n");
            }
            richTextBox2.Text = mainSb.ToString();
        }

        private void richTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!e.Control || e.KeyCode != Keys.V) return;
            ((RichTextBox)sender).Paste(DataFormats.GetFormat("Text"));
            e.Handled = true;
        }
    }
}