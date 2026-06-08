use interoptopus::{Interop, Error};
use interoptopus_backend_cpython::{Generator, Config};

#[test]
fn bindings_py() -> Result<(), Error> {
    Generator::new(
        Config {
            ..Config::default()
        },
        akatsuki_pp_ffi::my_inventory(),
    )
    .write_file("bindings/akatsuki_pp_ffi.py")?;

    Ok(())
}
