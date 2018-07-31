
$(function () {
    $('#uploadForm').on('submit', function (e) {
        e.preventDefault();

        var formData = new FormData($(this)[0]);
        $.ajax({
            url: 'UploadFiles',
            type: 'POST',
            data: formData,
            async: false,
            success: function (data) {
                xmlCompare(data.xmlData, siteMapOriginal);
            },
            cache: false,
            contentType: false,
            processData: false
        })
    });

    function xmlCompare(siteMapArray, originalData) {
        console.log('success', siteMapArray);
        var modifiedArray = [];
        if (originalData.length === 0) {
            siteMapArray.forEach(site => {
                site.status = 'new';
                modifiedArray.push(site);
            });
            return modifiedArray;
        }

        // check the original data to help set the flag of the siteMapArray for status
        siteMapArray.forEach(sitemap => {
            string status = 'new';//??? why do you use type constant in here?

            originalData.forEach(originalSite => {
                if (sitemap.url !== originalSite.Url) return;
                originalSite.status = 'edit';
                status = 'edit';
            })

            sitemap.status = status;
            modifiedArray.push(sitemap);
        })
        console.log('MODIFIED ARRAY', modifiedArray);
    }

});
