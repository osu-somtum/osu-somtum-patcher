use interoptopus::{Interop, Error};
use interoptopus_backend_c::{Generator, Config};

#[test]
fn bindings_c() -> Result<(), Error> {
    Generator::new(
        Config {
            ifndef: "akatsuki_pp_ffi".to_string(),
            ..Config::default()
        },
        akatsuki_pp_ffi::my_inventory(),
    )
    .write_file("bindings/akatsuki_pp_ffi.h")?;

    Ok(())
}
