if (DEFINED HOTROD_BUILD_BUNDLE)
  #Construct the bundle package name from the .msi package name.
  file (GLOB HOTROD_MSI_PACKAGE_PATH "*.msi")
  get_filename_component(HOTROD_MSI_PACKAGE "${HOTROD_MSI_PACKAGE_PATH}" NAME)
  string(REPLACE ".msi" "-bundle.exe" HOTROD_BUNDLE_PACKAGE "${HOTROD_MSI_PACKAGE}")
  
  include (CPackWIX)
  
  #Explicitly call WiX to build the bundle.
  execute_process(COMMAND "${CPACK_WIX_CANDLE_EXECUTABLE}" wix-bundle.xml -ext WixBalExtension)
  execute_process(COMMAND "${CPACK_WIX_LIGHT_EXECUTABLE}" -o "${HOTROD_BUNDLE_PACKAGE}" wix-bundle.wixobj -ext WixBalExtension)
endif()