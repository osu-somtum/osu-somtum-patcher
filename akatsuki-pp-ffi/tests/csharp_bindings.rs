use interoptopus::{Interop, Error};
use interoptopus_backend_csharp::{Generator, Config};

#[test]
fn bindings_cs() -> Result<(), Error> {
    Generator::new(
        Config {
            dll_name: "akatsuki_pp_ffi".to_string(),
            ..Config::default()
        },
        akatsuki_pp_ffi::my_inventory(),
    )
    .write_file("bindings/akatsuki_pp_ffi.cs")?;

    Ok(())
}
