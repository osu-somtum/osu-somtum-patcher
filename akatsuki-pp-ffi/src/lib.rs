use akatsuki_pp_1_0_1::{ // bancho.py server usually uses this version
    osu_2019::{stars::OsuPerformanceAttributes as AkatsukiOsuPerfAttrs, OsuPP as AkatsukiOsuPP},
    AnyPP as AkatsukiAnyPP, Beatmap as AkatsukiBeatmap, GameMode as AkatsukiGameMode,
    PerformanceAttributes as AkatsukiPerfAttrs,
};
use interoptopus::{
    extra_type, ffi_function, ffi_type, function,
    patterns::{option::FFIOption, slice::FFISlice},
    Inventory, InventoryBuilder,
};

#[ffi_type]
#[repr(C)]
#[derive(Clone, Default, PartialEq)]
pub struct CalculatePerformanceResult {
    pub pp: f64,
    pub stars: f64,
}

impl std::fmt::Display for CalculatePerformanceResult {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        write!(f, "pp: {}, stars: {}", self.pp, self.stars)
    }
}

trait IntoResult {
    fn into_result(self) -> CalculatePerformanceResult;
}

impl IntoResult for AkatsukiPerfAttrs {
    fn into_result(self) -> CalculatePerformanceResult {
        CalculatePerformanceResult { pp: self.pp(), stars: self.stars() }
    }
}

impl IntoResult for AkatsukiOsuPerfAttrs {
    fn into_result(self) -> CalculatePerformanceResult {
        CalculatePerformanceResult { pp: self.pp, stars: self.difficulty.stars }
    }
}

fn calculate_akatsuki_performance(
    beatmap: &AkatsukiBeatmap,
    mode: u32,
    mods: u32,
    max_combo: u32,
    accuracy: f32,
    miss_count: u32,
    passed_objects: Option<u32>,
) -> CalculatePerformanceResult {
    if mode == 0 && (mods & 128) != 0 {
        let mut calc = AkatsukiOsuPP::new(beatmap)
            .mods(mods)
            .combo(max_combo as usize)
            .misses(miss_count as usize);

        if let Some(passed) = passed_objects {
            calc = calc.passed_objects(passed as usize);
        }

        calc = calc.accuracy(accuracy);

        calc.calculate().into_result()
    } else {
        let game_mode = match mode {
            0 => AkatsukiGameMode::Osu,
            1 => AkatsukiGameMode::Taiko,
            2 => AkatsukiGameMode::Catch,
            3 => AkatsukiGameMode::Mania,
            _ => panic!("Invalid mode: {}", mode),
        };

        let mut calc = AkatsukiAnyPP::new(beatmap)
            .mode(game_mode)
            .mods(mods)
            .combo(max_combo as usize)
            .n_misses(miss_count as usize)
            .accuracy(accuracy as f64);

        if let Some(passed) = passed_objects {
            calc = calc.passed_objects(passed as usize);
        }

        calc.calculate().into_result()
    }
}

#[ffi_function]
#[no_mangle]
pub unsafe extern "C" fn calculate_akatsuki_from_bytes(
    beatmap_bytes: FFISlice<u8>,
    mode: u32,
    mods: u32,
    max_combo: u32,
    accuracy: f32,
    miss_count: u32,
    passed_objects: FFIOption<u32>,
) -> CalculatePerformanceResult {
    let beatmap = AkatsukiBeatmap::from_bytes(beatmap_bytes.as_slice()).unwrap();

    calculate_akatsuki_performance(
        &beatmap,
        mode,
        mods,
        max_combo,
        accuracy,
        miss_count,
        passed_objects.into_option(),
    )
}

pub fn my_inventory() -> Inventory {
    InventoryBuilder::new()
        .register(extra_type!(CalculatePerformanceResult))
        .register(function!(calculate_akatsuki_from_bytes))
        .inventory()
}
