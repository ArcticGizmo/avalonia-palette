using Palette.Theming.Color;

namespace Palette.Theming;

/// <summary>
/// The built-in palettes: six families, each with a light and a dark variant, tuned for
/// long editing sessions and (where the underlying scheme allows) WCAG AA on all text and
/// syntax pairings. See <c>docs/palette-rationale.md</c> for the reasoning and sources
/// behind each family.
/// </summary>
public static class PaletteCatalog
{
    private static Rgb H(string hex) => Rgb.FromHex(hex);

    /// <summary>The palette applied at startup (the author's house dark scheme).</summary>
    public static PaletteDefinition Default => AuroraDark;

    // ═══════════════════════════════════════════════════════════════════════
    //  Aurora — the author's house palette (#181820 / #60A5FA), shared across
    //  sprig / perch / emuwren. Balanced modern dark + a crafted daylight pair.
    // ═══════════════════════════════════════════════════════════════════════
    public static readonly PaletteDefinition AuroraDark = new()
    {
        Id = "aurora-dark", Name = "Aurora", Family = "Aurora", Variant = PaletteVariant.Dark,
        Description = "The house dark scheme — deep indigo surfaces, sky-blue accent.",
        Surface = H("#181820"), SurfaceSunken = H("#121218"), SurfaceRaised = H("#1F1F2A"),
        Overlay = H("#0F0F14"), Border = H("#2D2D3C"), Separator = H("#23232F"), ButtonBg = H("#2D2D3C"),
        TextPrimary = H("#E1E1EB"), TextTitle = H("#F5F5FA"), TextMuted = H("#8C8CA0"), TextFaint = H("#65657A"),
        Accent = H("#60A5FA"), AccentHover = H("#93C5FD"), OnAccent = H("#0B1220"), Link = H("#93C5FD"),
        Success = H("#4ADE80"), Warning = H("#FBBF24"), Danger = H("#F87171"), Info = H("#60A5FA"), Dev = H("#FF5FB0"),
        EditorBg = H("#14141C"), EditorFg = H("#E1E1EB"), EditorGutterFg = H("#565669"),
        Keyword = H("#C792EA"), Str = H("#A5D6A7"), Number = H("#F5B970"), Comment = H("#8A91A6"),
        Function = H("#82AAFF"), Type = H("#7FD1C4"), Variable = H("#E1E1EB"), Operator = H("#89DDFF"),
        Constant = H("#F78C6C"), Tag = H("#F07178"), Attribute = H("#FFCB6B"), Punctuation = H("#A6ACCD"),
        DiffAddedText = H("#4ADE80"), DiffRemovedText = H("#F87171"),
    };

    public static readonly PaletteDefinition AuroraLight = new()
    {
        Id = "aurora-light", Name = "Aurora", Family = "Aurora", Variant = PaletteVariant.Light,
        Description = "Daybreak — the house scheme in daylight: cool paper, royal-blue accent.",
        Surface = H("#F7F8FA"), SurfaceSunken = H("#EEF0F4"), SurfaceRaised = H("#FFFFFF"),
        Overlay = H("#FFFFFF"), Border = H("#D6DAE3"), Separator = H("#E4E7EE"), ButtonBg = H("#EAECF2"),
        TextPrimary = H("#22252B"), TextTitle = H("#0F1115"), TextMuted = H("#586173"), TextFaint = H("#8A909E"),
        Accent = H("#2563EB"), AccentHover = H("#1D4ED8"), OnAccent = H("#FFFFFF"), Link = H("#1D4ED8"),
        Success = H("#15803D"), Warning = H("#B45309"), Danger = H("#B91C1C"), Info = H("#2563EB"), Dev = H("#BE185D"),
        EditorBg = H("#FFFFFF"), EditorFg = H("#24292F"), EditorGutterFg = H("#8A909E"),
        Keyword = H("#CF222E"), Str = H("#0A3069"), Number = H("#0550AE"), Comment = H("#57606A"),
        Function = H("#8250DF"), Type = H("#953800"), Variable = H("#24292F"), Operator = H("#0550AE"),
        Constant = H("#0550AE"), Tag = H("#116329"), Attribute = H("#0550AE"), Punctuation = H("#24292F"),
        DiffAddedText = H("#1A7F37"), DiffRemovedText = H("#CF222E"),
    };

    // ═══════════════════════════════════════════════════════════════════════
    //  Solarized — Ethan Schoonover. Low brightness-contrast, high hue-contrast.
    //  Comment/body values nudged to reach AA (canonical Solarized fails there).
    // ═══════════════════════════════════════════════════════════════════════
    public static readonly PaletteDefinition SolarizedDark = new()
    {
        Id = "solarized-dark", Name = "Solarized", Family = "Solarized", Variant = PaletteVariant.Dark,
        Description = "Teal-toned dark; famed for reduced brightness contrast over long sessions.",
        Surface = H("#002B36"), SurfaceSunken = H("#00252E"), SurfaceRaised = H("#073642"),
        Overlay = H("#00252E"), Border = H("#0E4B5A"), Separator = H("#0A3F4C"), ButtonBg = H("#073642"),
        TextPrimary = H("#93A1A1"), TextTitle = H("#CBD3D1"), TextMuted = H("#839496"), TextFaint = H("#657B83"),
        Accent = H("#268BD2"), AccentHover = H("#4DA3DE"), OnAccent = H("#01151B"), Link = H("#4DA3DE"),
        Success = H("#859900"), Warning = H("#B58900"), Danger = H("#F0605C"), Info = H("#268BD2"), Dev = H("#E265A0"),
        EditorBg = H("#002B36"), EditorFg = H("#93A1A1"), EditorGutterFg = H("#586E75"),
        Keyword = H("#859900"), Str = H("#2AA198"), Number = H("#E265A0"), Comment = H("#8AA0A6"),
        Function = H("#4DA3DE"), Type = H("#B58900"), Variable = H("#93A1A1"), Operator = H("#859900"),
        Constant = H("#E265A0"), Tag = H("#4DA3DE"), Attribute = H("#9AA0E0"), Punctuation = H("#93A1A1"),
        DiffAddedText = H("#859900"), DiffRemovedText = H("#F0605C"),
    };

    public static readonly PaletteDefinition SolarizedLight = new()
    {
        Id = "solarized-light", Name = "Solarized", Family = "Solarized", Variant = PaletteVariant.Light,
        Description = "Warm cream light companion to Solarized dark.",
        Surface = H("#FDF6E3"), SurfaceSunken = H("#F2EAD3"), SurfaceRaised = H("#FFFEF7"),
        Overlay = H("#FFFEF7"), Border = H("#E5DCC3"), Separator = H("#EDE4CC"), ButtonBg = H("#EEE8D5"),
        TextPrimary = H("#4E5F65"), TextTitle = H("#073642"), TextMuted = H("#5F7379"), TextFaint = H("#93A1A1"),
        Accent = H("#1C6394"), AccentHover = H("#14507A"), OnAccent = H("#FDF6E3"), Link = H("#14507A"),
        Success = H("#5C6B00"), Warning = H("#8A6D00"), Danger = H("#C42B28"), Info = H("#14507A"), Dev = H("#B02A6B"),
        EditorBg = H("#FDF6E3"), EditorFg = H("#4E5F65"), EditorGutterFg = H("#93A1A1"),
        Keyword = H("#5E7000"), Str = H("#1A776E"), Number = H("#A02A66"), Comment = H("#5F7379"),
        Function = H("#1E6FA8"), Type = H("#8A6800"), Variable = H("#4E5F65"), Operator = H("#5E7000"),
        Constant = H("#A02A66"), Tag = H("#1E6FA8"), Attribute = H("#4F53A8"), Punctuation = H("#4E5F65"),
        DiffAddedText = H("#5E7000"), DiffRemovedText = H("#C42B28"),
    };

    // ═══════════════════════════════════════════════════════════════════════
    //  Nord — arcticicestudio. Low-saturation arctic blues; calm, cool.
    // ═══════════════════════════════════════════════════════════════════════
    public static readonly PaletteDefinition NordDark = new()
    {
        Id = "nord-dark", Name = "Nord", Family = "Nord", Variant = PaletteVariant.Dark,
        Description = "Polar Night — desaturated blue-grey; low glare, low colour clash.",
        Surface = H("#2E3440"), SurfaceSunken = H("#292E39"), SurfaceRaised = H("#3B4252"),
        Overlay = H("#2B303B"), Border = H("#434C5E"), Separator = H("#3B4252"), ButtonBg = H("#434C5E"),
        TextPrimary = H("#D8DEE9"), TextTitle = H("#ECEFF4"), TextMuted = H("#A7B0C0"), TextFaint = H("#6E7789"),
        Accent = H("#88C0D0"), AccentHover = H("#8FBCBB"), OnAccent = H("#2E3440"), Link = H("#88C0D0"),
        Success = H("#A3BE8C"), Warning = H("#EBCB8B"), Danger = H("#DA8890"), Info = H("#81A1C1"), Dev = H("#C79CC2"),
        EditorBg = H("#2E3440"), EditorFg = H("#D8DEE9"), EditorGutterFg = H("#616E88"),
        Keyword = H("#81A1C1"), Str = H("#A3BE8C"), Number = H("#C79CC2"), Comment = H("#99A4B8"),
        Function = H("#88C0D0"), Type = H("#8FBCBB"), Variable = H("#D8DEE9"), Operator = H("#81A1C1"),
        Constant = H("#C79CC2"), Tag = H("#81A1C1"), Attribute = H("#8FBCBB"), Punctuation = H("#ECEFF4"),
        DiffAddedText = H("#A3BE8C"), DiffRemovedText = H("#DA8890"),
    };

    public static readonly PaletteDefinition NordLight = new()
    {
        Id = "nord-light", Name = "Nord", Family = "Nord", Variant = PaletteVariant.Light,
        Description = "Snow Storm — Nord's light side; soft, cool paper.",
        Surface = H("#ECEFF4"), SurfaceSunken = H("#E1E6EF"), SurfaceRaised = H("#F4F6FA"),
        Overlay = H("#F4F6FA"), Border = H("#D2D9E6"), Separator = H("#DEE3EC"), ButtonBg = H("#E0E5EE"),
        TextPrimary = H("#2E3440"), TextTitle = H("#232833"), TextMuted = H("#4C566A"), TextFaint = H("#7A8393"),
        Accent = H("#4C6E97"), AccentHover = H("#3D5A80"), OnAccent = H("#ECEFF4"), Link = H("#3D5A80"),
        Success = H("#446A2E"), Warning = H("#7C6017"), Danger = H("#A54852"), Info = H("#4C6E97"), Dev = H("#8A5F84"),
        EditorBg = H("#FBFCFE"), EditorFg = H("#2E3440"), EditorGutterFg = H("#99A3B5"),
        Keyword = H("#42618F"), Str = H("#446A2E"), Number = H("#7A5A86"), Comment = H("#697488"),
        Function = H("#3E6A8C"), Type = H("#37716F"), Variable = H("#2E3440"), Operator = H("#42618F"),
        Constant = H("#7A5A86"), Tag = H("#42618F"), Attribute = H("#37716F"), Punctuation = H("#2E3440"),
        DiffAddedText = H("#446A2E"), DiffRemovedText = H("#A54852"),
    };

    // ═══════════════════════════════════════════════════════════════════════
    //  Gruvbox — morhetz. Warm, retro, reduced blue light; kind in the evening.
    // ═══════════════════════════════════════════════════════════════════════
    public static readonly PaletteDefinition GruvboxDark = new()
    {
        Id = "gruvbox-dark", Name = "Gruvbox", Family = "Gruvbox", Variant = PaletteVariant.Dark,
        Description = "Warm retro dark; low blue light for late sessions.",
        Surface = H("#282828"), SurfaceSunken = H("#1D2021"), SurfaceRaised = H("#32302F"),
        Overlay = H("#1D2021"), Border = H("#504945"), Separator = H("#3C3836"), ButtonBg = H("#504945"),
        TextPrimary = H("#EBDBB2"), TextTitle = H("#FBF1C7"), TextMuted = H("#A89984"), TextFaint = H("#7C6F64"),
        Accent = H("#83A598"), AccentHover = H("#A3C5B8"), OnAccent = H("#282828"), Link = H("#83A598"),
        Success = H("#B8BB26"), Warning = H("#FABD2F"), Danger = H("#FE5E49"), Info = H("#83A598"), Dev = H("#D3869B"),
        EditorBg = H("#282828"), EditorFg = H("#EBDBB2"), EditorGutterFg = H("#7C6F64"),
        Keyword = H("#FE5E49"), Str = H("#B8BB26"), Number = H("#D3869B"), Comment = H("#A89984"),
        Function = H("#B8BB26"), Type = H("#FABD2F"), Variable = H("#EBDBB2"), Operator = H("#8EC07C"),
        Constant = H("#D3869B"), Tag = H("#8EC07C"), Attribute = H("#FABD2F"), Punctuation = H("#EBDBB2"),
        DiffAddedText = H("#B8BB26"), DiffRemovedText = H("#FE5E49"),
    };

    public static readonly PaletteDefinition GruvboxLight = new()
    {
        Id = "gruvbox-light", Name = "Gruvbox", Family = "Gruvbox", Variant = PaletteVariant.Light,
        Description = "Warm cream light; retro and soft on the eyes.",
        Surface = H("#FBF1C7"), SurfaceSunken = H("#F2E5BC"), SurfaceRaised = H("#FFFBEF"),
        Overlay = H("#FFFBEF"), Border = H("#E0D3A8"), Separator = H("#EBDDB4"), ButtonBg = H("#EBDBB2"),
        TextPrimary = H("#3C3836"), TextTitle = H("#282828"), TextMuted = H("#665C54"), TextFaint = H("#928374"),
        Accent = H("#076678"), AccentHover = H("#054E5C"), OnAccent = H("#FBF1C7"), Link = H("#076678"),
        Success = H("#6A660C"), Warning = H("#8A5900"), Danger = H("#9D0006"), Info = H("#076678"), Dev = H("#8F3F71"),
        EditorBg = H("#FBF1C7"), EditorFg = H("#3C3836"), EditorGutterFg = H("#928374"),
        Keyword = H("#9D0006"), Str = H("#6A660C"), Number = H("#8F3F71"), Comment = H("#6E6257"),
        Function = H("#6A660C"), Type = H("#8F5D10"), Variable = H("#3C3836"), Operator = H("#427B58"),
        Constant = H("#8F3F71"), Tag = H("#427B58"), Attribute = H("#8F5D10"), Punctuation = H("#3C3836"),
        DiffAddedText = H("#6A660C"), DiffRemovedText = H("#9D0006"),
    };

    // ═══════════════════════════════════════════════════════════════════════
    //  Sepia — warm, low-blue-light "reading" pair. Rationale: reducing blue
    //  primaries lowers photoreceptor/melanopic stimulation in dim rooms
    //  (the basis of Night Shift / f.lux). Kept low-contrast and glare-free.
    // ═══════════════════════════════════════════════════════════════════════
    public static readonly PaletteDefinition SepiaDark = new()
    {
        Id = "sepia-dark", Name = "Sepia", Family = "Sepia", Variant = PaletteVariant.Dark,
        Description = "Ember — warm charcoal, amber accent, minimal blue for night work.",
        Surface = H("#211C18"), SurfaceSunken = H("#1A1512"), SurfaceRaised = H("#2B2420"),
        Overlay = H("#1A1512"), Border = H("#3E362F"), Separator = H("#322A24"), ButtonBg = H("#3E362F"),
        TextPrimary = H("#E6D8C3"), TextTitle = H("#F5EAD6"), TextMuted = H("#A99A85"), TextFaint = H("#7C6F5F"),
        Accent = H("#E0A458"), AccentHover = H("#EDBB79"), OnAccent = H("#241B10"), Link = H("#EDBB79"),
        Success = H("#A3B565"), Warning = H("#E0A458"), Danger = H("#E07A5F"), Info = H("#C99E6A"), Dev = H("#C98BA0"),
        EditorBg = H("#211C18"), EditorFg = H("#E6D8C3"), EditorGutterFg = H("#6E6152"),
        Keyword = H("#D98E73"), Str = H("#A9B573"), Number = H("#D6A86A"), Comment = H("#9A8B72"),
        Function = H("#E0B15C"), Type = H("#C9A97E"), Variable = H("#E6D8C3"), Operator = H("#CBA46A"),
        Constant = H("#D98E73"), Tag = H("#A9B573"), Attribute = H("#D6A86A"), Punctuation = H("#C9BBA3"),
        DiffAddedText = H("#A3B565"), DiffRemovedText = H("#E07A5F"),
    };

    public static readonly PaletteDefinition SepiaLight = new()
    {
        Id = "sepia-light", Name = "Sepia", Family = "Sepia", Variant = PaletteVariant.Light,
        Description = "Parchment — paper-warm light; softer than white in bright rooms.",
        Surface = H("#F4ECD8"), SurfaceSunken = H("#EBE0C6"), SurfaceRaised = H("#FBF6E7"),
        Overlay = H("#FBF6E7"), Border = H("#DDD0AE"), Separator = H("#E7DBBE"), ButtonBg = H("#EADFC4"),
        TextPrimary = H("#433A2B"), TextTitle = H("#2C2418"), TextMuted = H("#6E6047"), TextFaint = H("#94856A"),
        Accent = H("#A5591A"), AccentHover = H("#8F4F14"), OnAccent = H("#FBF6E7"), Link = H("#8F4F14"),
        Success = H("#5C6E1E"), Warning = H("#8A5900"), Danger = H("#A63A28"), Info = H("#7A5A1E"), Dev = H("#8A4A66"),
        EditorBg = H("#FBF6E7"), EditorFg = H("#433A2B"), EditorGutterFg = H("#94856A"),
        Keyword = H("#9A3B1E"), Str = H("#5C6E1E"), Number = H("#7A4E86"), Comment = H("#736349"),
        Function = H("#8A5A12"), Type = H("#6E5A1E"), Variable = H("#433A2B"), Operator = H("#4E6E52"),
        Constant = H("#7A4E86"), Tag = H("#4E6E52"), Attribute = H("#8A5A12"), Punctuation = H("#433A2B"),
        DiffAddedText = H("#5C6E1E"), DiffRemovedText = H("#A63A28"),
    };

    // ═══════════════════════════════════════════════════════════════════════
    //  High Contrast — accessibility-first, targeting WCAG AAA (≥ 7:1) on text.
    //  Dark avoids pure black (#000) to curb halation; light avoids pure noise.
    // ═══════════════════════════════════════════════════════════════════════
    public static readonly PaletteDefinition ContrastDark = new()
    {
        Id = "contrast-dark", Name = "High Contrast", Family = "High Contrast", Variant = PaletteVariant.Dark,
        Description = "AAA-targeted dark; maximum legibility, softened near-black to reduce glare.",
        Surface = H("#0A0A0C"), SurfaceSunken = H("#050506"), SurfaceRaised = H("#16161A"),
        Overlay = H("#050506"), Border = H("#4A4A52"), Separator = H("#33333A"), ButtonBg = H("#24242A"),
        TextPrimary = H("#F4F4F7"), TextTitle = H("#FFFFFF"), TextMuted = H("#C2C2CC"), TextFaint = H("#9A9AA6"),
        Accent = H("#7CB3FF"), AccentHover = H("#A6CCFF"), OnAccent = H("#05060A"), Link = H("#A6CCFF"),
        Success = H("#5EE38A"), Warning = H("#FFD23F"), Danger = H("#FF8A8A"), Info = H("#7CB3FF"), Dev = H("#FF8AD0"),
        EditorBg = H("#0A0A0C"), EditorFg = H("#F4F4F7"), EditorGutterFg = H("#B8B8C4"),
        Keyword = H("#E0A3FF"), Str = H("#A9E6A0"), Number = H("#FFC266"), Comment = H("#B0B0BE"),
        Function = H("#9DC1FF"), Type = H("#86E0D0"), Variable = H("#F4F4F7"), Operator = H("#8EE6FF"),
        Constant = H("#FFAE8A"), Tag = H("#FF9AA0"), Attribute = H("#FFDE8A"), Punctuation = H("#D6D6E0"),
        DiffAddedText = H("#5EE38A"), DiffRemovedText = H("#FF8A8A"),
    };

    public static readonly PaletteDefinition ContrastLight = new()
    {
        Id = "contrast-light", Name = "High Contrast", Family = "High Contrast", Variant = PaletteVariant.Light,
        Description = "AAA-targeted light; near-black ink on white for bright environments.",
        Surface = H("#FFFFFF"), SurfaceSunken = H("#F0F1F3"), SurfaceRaised = H("#FFFFFF"),
        Overlay = H("#FFFFFF"), Border = H("#8A8F99"), Separator = H("#C9CDD4"), ButtonBg = H("#E9EBEF"),
        TextPrimary = H("#0A0C10"), TextTitle = H("#000000"), TextMuted = H("#45484F"), TextFaint = H("#6A6E77"),
        Accent = H("#0B5CD6"), AccentHover = H("#06429E"), OnAccent = H("#FFFFFF"), Link = H("#06429E"),
        Success = H("#146C2E"), Warning = H("#8A5300"), Danger = H("#B00020"), Info = H("#0B5CD6"), Dev = H("#9C1458"),
        EditorBg = H("#FFFFFF"), EditorFg = H("#0A0C10"), EditorGutterFg = H("#5A5E67"),
        Keyword = H("#A21133"), Str = H("#0A5A2E"), Number = H("#063E9E"), Comment = H("#4A4E57"),
        Function = H("#6A1B9A"), Type = H("#8A4300"), Variable = H("#0A0C10"), Operator = H("#A21133"),
        Constant = H("#063E9E"), Tag = H("#0A5A2E"), Attribute = H("#063E9E"), Punctuation = H("#0A0C10"),
        DiffAddedText = H("#0A5A2E"), DiffRemovedText = H("#A21133"),
    };

    // ═══════════════════════════════════════════════════════════════════════
    //  One — Atom's balanced classic. Neutral surfaces, wide syntax hue spread.
    // ═══════════════════════════════════════════════════════════════════════
    public static readonly PaletteDefinition OneDark = new()
    {
        Id = "one-dark", Name = "One", Family = "One", Variant = PaletteVariant.Dark,
        Description = "Atom's One Dark — balanced slate with a broad syntax palette.",
        Surface = H("#282C34"), SurfaceSunken = H("#21252B"), SurfaceRaised = H("#2C313A"),
        Overlay = H("#21252B"), Border = H("#3E4451"), Separator = H("#333842"), ButtonBg = H("#3E4451"),
        TextPrimary = H("#ABB2BF"), TextTitle = H("#D7DAE0"), TextMuted = H("#949BA8"), TextFaint = H("#5C6370"),
        Accent = H("#61AFEF"), AccentHover = H("#82C0F5"), OnAccent = H("#21252B"), Link = H("#61AFEF"),
        Success = H("#98C379"), Warning = H("#E5C07B"), Danger = H("#E97F87"), Info = H("#61AFEF"), Dev = H("#C678DD"),
        EditorBg = H("#282C34"), EditorFg = H("#ABB2BF"), EditorGutterFg = H("#5C6370"),
        Keyword = H("#C678DD"), Str = H("#98C379"), Number = H("#D19A66"), Comment = H("#8A93A2"),
        Function = H("#61AFEF"), Type = H("#E5C07B"), Variable = H("#E97F87"), Operator = H("#56B6C2"),
        Constant = H("#D19A66"), Tag = H("#E97F87"), Attribute = H("#D19A66"), Punctuation = H("#ABB2BF"),
        DiffAddedText = H("#98C379"), DiffRemovedText = H("#E97F87"),
    };

    public static readonly PaletteDefinition OneLight = new()
    {
        Id = "one-light", Name = "One", Family = "One", Variant = PaletteVariant.Light,
        Description = "Atom's One Light — clean neutral paper.",
        Surface = H("#FAFAFA"), SurfaceSunken = H("#EAEAEB"), SurfaceRaised = H("#FFFFFF"),
        Overlay = H("#FFFFFF"), Border = H("#D4D4D6"), Separator = H("#E4E4E6"), ButtonBg = H("#ECECEE"),
        TextPrimary = H("#383A42"), TextTitle = H("#1F2024"), TextMuted = H("#6A6C74"), TextFaint = H("#A0A1A7"),
        Accent = H("#295AC4"), AccentHover = H("#1E469E"), OnAccent = H("#FFFFFF"), Link = H("#1E469E"),
        Success = H("#2F6B2E"), Warning = H("#986801"), Danger = H("#C42B1C"), Info = H("#295AC4"), Dev = H("#A626A4"),
        EditorBg = H("#FFFFFF"), EditorFg = H("#383A42"), EditorGutterFg = H("#A0A1A7"),
        Keyword = H("#A626A4"), Str = H("#2F6B2E"), Number = H("#986801"), Comment = H("#6E6F76"),
        Function = H("#295AC4"), Type = H("#986801"), Variable = H("#383A42"), Operator = H("#0A6C93"),
        Constant = H("#986801"), Tag = H("#2F6B2E"), Attribute = H("#986801"), Punctuation = H("#383A42"),
        DiffAddedText = H("#2F6B2E"), DiffRemovedText = H("#C42B1C"),
    };

    // ═══════════════════════════════════════════════════════════════════════
    //  Tokyo Night — deep indigo dark + a soft "Day" companion.
    // ═══════════════════════════════════════════════════════════════════════
    public static readonly PaletteDefinition TokyoNightDark = new()
    {
        Id = "tokyonight-dark", Name = "Tokyo Night", Family = "Tokyo Night", Variant = PaletteVariant.Dark,
        Description = "Deep indigo night; saturated but low-glare.",
        Surface = H("#1A1B26"), SurfaceSunken = H("#16161E"), SurfaceRaised = H("#24283B"),
        Overlay = H("#16161E"), Border = H("#2E3350"), Separator = H("#24283B"), ButtonBg = H("#2E3350"),
        TextPrimary = H("#C0CAF5"), TextTitle = H("#D5DBF5"), TextMuted = H("#9AA5CE"), TextFaint = H("#565F89"),
        Accent = H("#7AA2F7"), AccentHover = H("#A2BEF9"), OnAccent = H("#16161E"), Link = H("#7AA2F7"),
        Success = H("#9ECE6A"), Warning = H("#E0AF68"), Danger = H("#F7768E"), Info = H("#7DCFFF"), Dev = H("#BB9AF7"),
        EditorBg = H("#1A1B26"), EditorFg = H("#C0CAF5"), EditorGutterFg = H("#565F89"),
        Keyword = H("#BB9AF7"), Str = H("#9ECE6A"), Number = H("#FF9E64"), Comment = H("#838CBB"),
        Function = H("#7AA2F7"), Type = H("#73DACA"), Variable = H("#C0CAF5"), Operator = H("#7DCFFF"),
        Constant = H("#FF9E64"), Tag = H("#F7768E"), Attribute = H("#E0AF68"), Punctuation = H("#A9B1D6"),
        DiffAddedText = H("#9ECE6A"), DiffRemovedText = H("#F7768E"),
    };

    public static readonly PaletteDefinition TokyoNightLight = new()
    {
        Id = "tokyonight-light", Name = "Tokyo Night", Family = "Tokyo Night", Variant = PaletteVariant.Light,
        Description = "Tokyo Night Day — muted cool paper companion.",
        Surface = H("#E1E2E7"), SurfaceSunken = H("#D4D6E0"), SurfaceRaised = H("#EDEEF2"),
        Overlay = H("#EDEEF2"), Border = H("#C4C8D8"), Separator = H("#D4D6E0"), ButtonBg = H("#D8DAE6"),
        TextPrimary = H("#2C304D"), TextTitle = H("#1A1D33"), TextMuted = H("#565A7E"), TextFaint = H("#848CB5"),
        Accent = H("#1E63C4"), AccentHover = H("#164D9E"), OnAccent = H("#FFFFFF"), Link = H("#164D9E"),
        Success = H("#4C6630"), Warning = H("#79591F"), Danger = H("#B02749"), Info = H("#007197"), Dev = H("#7A3EC0"),
        EditorBg = H("#F7F8FB"), EditorFg = H("#2C304D"), EditorGutterFg = H("#848CB5"),
        Keyword = H("#7A3EC0"), Str = H("#587539"), Number = H("#A15400"), Comment = H("#616684"),
        Function = H("#1E63C4"), Type = H("#0C6E5B"), Variable = H("#2C304D"), Operator = H("#007197"),
        Constant = H("#B15C00"), Tag = H("#C22F52"), Attribute = H("#8C6C3E"), Punctuation = H("#2C304D"),
        DiffAddedText = H("#587539"), DiffRemovedText = H("#C22F52"),
    };

    // ═══════════════════════════════════════════════════════════════════════
    //  Rosé Pine — soft, low-contrast and aesthetic; "main" dark + "Dawn" light.
    // ═══════════════════════════════════════════════════════════════════════
    public static readonly PaletteDefinition RosePineDark = new()
    {
        Id = "rosepine-dark", Name = "Rosé Pine", Family = "Rosé Pine", Variant = PaletteVariant.Dark,
        Description = "Muted rose-and-pine dark; gentle, low-contrast, cosy.",
        Surface = H("#191724"), SurfaceSunken = H("#14121F"), SurfaceRaised = H("#1F1D2E"),
        Overlay = H("#26233A"), Border = H("#2E2B40"), Separator = H("#26233A"), ButtonBg = H("#2E2B40"),
        TextPrimary = H("#E0DEF4"), TextTitle = H("#F0EFFA"), TextMuted = H("#A9A5C4"), TextFaint = H("#6E6A86"),
        Accent = H("#C4A7E7"), AccentHover = H("#D6C2F0"), OnAccent = H("#191724"), Link = H("#C4A7E7"),
        Success = H("#9CCFD8"), Warning = H("#F6C177"), Danger = H("#EB6F92"), Info = H("#9CCFD8"), Dev = H("#C4A7E7"),
        EditorBg = H("#191724"), EditorFg = H("#E0DEF4"), EditorGutterFg = H("#6E6A86"),
        Keyword = H("#6F9DB9"), Str = H("#F6C177"), Number = H("#EBBCBA"), Comment = H("#8A87A3"),
        Function = H("#9CCFD8"), Type = H("#C4A7E7"), Variable = H("#E0DEF4"), Operator = H("#9CCFD8"),
        Constant = H("#F6C177"), Tag = H("#EB6F92"), Attribute = H("#F6C177"), Punctuation = H("#A9A5C4"),
        DiffAddedText = H("#9CCFD8"), DiffRemovedText = H("#EB6F92"),
    };

    public static readonly PaletteDefinition RosePineLight = new()
    {
        Id = "rosepine-light", Name = "Rosé Pine", Family = "Rosé Pine", Variant = PaletteVariant.Light,
        Description = "Rosé Pine Dawn — warm blush light; soft and unhurried.",
        Surface = H("#FAF4ED"), SurfaceSunken = H("#F2E9E1"), SurfaceRaised = H("#FFFAF3"),
        Overlay = H("#F2E9E1"), Border = H("#E4D8CC"), Separator = H("#EFE6DC"), ButtonBg = H("#EFE0D4"),
        TextPrimary = H("#575279"), TextTitle = H("#3F3A5C"), TextMuted = H("#6E6A86"), TextFaint = H("#9893A5"),
        Accent = H("#75608F"), AccentHover = H("#5C4A73"), OnAccent = H("#FFFAF3"), Link = H("#5C4A73"),
        Success = H("#3D7883"), Warning = H("#8A6510"), Danger = H("#9E4A62"), Info = H("#286983"), Dev = H("#7A5E96"),
        EditorBg = H("#FFFAF3"), EditorFg = H("#575279"), EditorGutterFg = H("#9893A5"),
        Keyword = H("#286983"), Str = H("#8A6510"), Number = H("#9E4A62"), Comment = H("#6E6A86"),
        Function = H("#3D7883"), Type = H("#7A5E96"), Variable = H("#575279"), Operator = H("#3D7883"),
        Constant = H("#8A6510"), Tag = H("#9E4A62"), Attribute = H("#8A6510"), Punctuation = H("#575279"),
        DiffAddedText = H("#3D7883"), DiffRemovedText = H("#9E4A62"),
    };

    /// <summary>Every built-in palette, grouped light/dark by family in menu order.</summary>
    public static readonly IReadOnlyList<PaletteDefinition> All = new[]
    {
        AuroraDark, AuroraLight,
        SolarizedDark, SolarizedLight,
        NordDark, NordLight,
        GruvboxDark, GruvboxLight,
        OneDark, OneLight,
        TokyoNightDark, TokyoNightLight,
        RosePineDark, RosePineLight,
        SepiaDark, SepiaLight,
        ContrastDark, ContrastLight,
    };

    /// <summary>Look a palette up by its <see cref="PaletteDefinition.Id"/>.</summary>
    public static PaletteDefinition ById(string id) =>
        All.FirstOrDefault(p => string.Equals(p.Id, id, StringComparison.OrdinalIgnoreCase))
        ?? throw new KeyNotFoundException($"No palette with id '{id}'.");

    /// <summary>The dark and light variants for a family name.</summary>
    public static IEnumerable<PaletteDefinition> Family(string family) =>
        All.Where(p => string.Equals(p.Family, family, StringComparison.OrdinalIgnoreCase));
}
