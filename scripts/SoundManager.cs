using Godot;
using System;

public class SoundManager : Node
{
    public static SoundManager Singleton;

    // Ambient nodes
    AudioStreamPlayer Day => _day ??= GetNode<AudioStreamPlayer>("AmbientDay"); AudioStreamPlayer _day;
    AudioStreamPlayer Night => _night ??= GetNode<AudioStreamPlayer>("AmbientNight"); AudioStreamPlayer _night;
    AudioStreamPlayer Office => _office ??= GetNode<AudioStreamPlayer>("AmbientOffice"); AudioStreamPlayer _office;
    AudioStreamPlayer Menu => _menu ??= GetNode<AudioStreamPlayer>("AmbientMenu"); AudioStreamPlayer _menu;

    // SFX nodes
    AudioStreamPlayer Hover => _hover ??= GetNode<AudioStreamPlayer>("SFXHover"); AudioStreamPlayer _hover;
    AudioStreamPlayer Select => _select ??= GetNode<AudioStreamPlayer>("SFXSelect"); AudioStreamPlayer _select;
    AudioStreamPlayer Scream => _scream ??= GetNode<AudioStreamPlayer>("SFXScream"); AudioStreamPlayer _scream;
    AudioStreamPlayer GrabBig => _grabBig ??= GetNode<AudioStreamPlayer>("SFXGrabBig"); AudioStreamPlayer _grabBig;
    AudioStreamPlayer DropBig => _dropBig ??= GetNode<AudioStreamPlayer>("SFXDropBig"); AudioStreamPlayer _dropBig;
    AudioStreamPlayer Switch => _switch ??= GetNode<AudioStreamPlayer>("SFXSwitch"); AudioStreamPlayer _switch;
    AudioStreamPlayer PoorLiquid => _poorLiquid ??= GetNode<AudioStreamPlayer>("SFXPoorLiquid"); AudioStreamPlayer _poorLiquid;

    public override void _Ready()
    {
        GD.Print($"INITIALIZING {GetType().Name}");

        // Handle the singleton on the C# side. GDScript side can access via an
        // Autoload property.
        if (Singleton == null)
        {
            Singleton = this;
        }
        else
        {
            throw new Exception($"Can't have more than one {GetType().Name}");
        }
    }

    public void ChangeAmbientSound(string sound = "None")
    {
        AmbientSoundType type = (AmbientSoundType)Enum.Parse(typeof(AmbientSoundType), sound);
        GD.Print($"changing ambient to {type}");

        Day.Playing = type == AmbientSoundType.Day;
        Night.Playing = type == AmbientSoundType.Night;
        Office.Playing = type == AmbientSoundType.Office;
        Menu.Playing = type == AmbientSoundType.Menu;
    }

    public void PlayOneShot(string sound = "Invalid")
    {
        SFXSoundType type = Enum.TryParse(sound, out SFXSoundType value)
            ? value
            : SFXSoundType.Invalid;

        PlayOneShot_typed(type);
    }

    public void PlayOneShot_typed(SFXSoundType sound)
    {
        if (sound == SFXSoundType.Invalid)
        {
            GD.Print($"missing or invalid SFX: {sound}");
        }

        AudioStreamPlayer sfx = sound switch
        {
            SFXSoundType.Hover => Hover,
            SFXSoundType.Select => Select,
            SFXSoundType.Scream => Scream,
            SFXSoundType.GrabBig => GrabBig,
            SFXSoundType.DropBig => DropBig,
            SFXSoundType.Switch => Switch,
            SFXSoundType.PoorLiquid => PoorLiquid,
            _ => Hover,
        };

        sfx.Play();
    }
}

public enum AmbientSoundType
{
    None = default,
    Menu,
    Day,
    Night,
    Office,
}

public enum SFXSoundType
{
    Invalid = default,
    Hover,
    Select,
    Scream,
    GrabBig,
    DropBig,
    Switch,
    PoorLiquid,
}
