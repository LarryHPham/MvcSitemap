
$(function () {
    $('#uploadForm').on('submit', function (e) {
        e.preventDefault();

        var formData = new FormData($(this)[0]);
        console.log(formData);

        $.ajax({
            url: 'UploadFiles',
            type: 'POST',
            data: formData,
            async: false,
            success: function (data) {
                console.log('success', data);
            },
            cache: false,
            contentType: false,
            processData: false
        })
    });
});
