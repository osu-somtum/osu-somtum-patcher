using System;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using OsuPatcher.Runtime.Constants;
using OsuPatcher.Runtime.Helpers;
using OsuPatcher.Runtime.Utils;

namespace OsuPatcher.Runtime.Options
{
    /// <summary>
    /// Manages the configuration options for the patcher.
    /// </summary>
    internal class Options
    {
        public static readonly Config Config = Config._load();

        /// <summary>
        /// Initializes the options menu with checkboxes and categories.
        /// </summary>
        /// <param name="instance">The instance of the options menu.</param>
        public static void InitializeOptions(object instance)
        {
            CheckBox alwaysShowMisses = new CheckBox("Patch Relax/Autopilot",
                "Removes relax/autopilot limitation, Allows you to see miss, Combo break sound and ranking panel.",
                Config.PatchRelax,
                Config.TogglePatchRelax);

            CheckBox transitionTime = new CheckBox("Faster Transition time",
                "Control how fast the screen fades in and out. Turn this on for quicker transitions.",
                Config.TransitionTime,
                Config.ToggleTransitionTime);

            CheckBox performanceCalculator = new CheckBox("Performance Calculator",
                "Let the patcher calculates your plays ingame.",
                Config.PerformanceCalculator,
                Config.TogglePerformanceCalculator);

            Array optionsChildren = Element.CreateArray(
                alwaysShowMisses,
                transitionTime,
                performanceCalculator);

            Section section = new Section("Patches");
            section.SetChildren(optionsChildren);

            Section credit = new Section("osu!somtum patcher v1.0 — somtum.fun");

            Array sectionChildren = Element.CreateArray(section, credit);
            
            Category category = new Category(FontAwesome.Magic);
            category.SetChildren(sectionChildren);

            // add to elements
            Add(instance, category);
        }

        private static void Add(object instance, Element element)
            => BaseAddElement.Invoke(instance, new[] { element.V });

        private static readonly MethodBase BaseAddElement = ILPatch.FindMethodBySignature(Patterns.Options_AddElement);
    }
}
