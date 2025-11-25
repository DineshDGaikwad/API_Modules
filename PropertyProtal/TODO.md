# TODO: Add "All Properties" Segment to Manage Properties Component

## Tasks
- [x] Update manage-properties.ts: Extend PropertySegment to include 'all', add properties for allProperties array, searchQuery, currentPage, pageSize, totalProperties, and filters (status, city).
- [x] Add methods: loadAllProperties(), searchProperties(), filterProperties(), paginate(), editProperty(), deleteProperty().
- [x] Update loadProperties() to include all properties using forkJoin.
- [x] Update manage-properties.html: Add third tab for 'All', search input bar, filter dropdowns (status, city), pagination controls, and edit/delete buttons in the all section.
- [x] Update manage-properties.css: Minor adjustments for new elements.
- [x] Test the new functionality.
- [x] Ensure edit navigation works (router.navigate to property edit route).
- [x] Verify delete confirmation and API call.
