$(document).ready(function () {

    var current_fs, next_fs, previous_fs;
    var opacity;

    $(".next").click(function () {
        current_fs = $(this).closest("fieldset");
        var currentIndex = $("fieldset").index(current_fs);

        // Step 1: AJAX + validation
        if (currentIndex === 0) {
            var isValid = true;

            // Only validate inputs inside step1
            $('#step1 :input').each(function () {
                if (!$(this).valid()) {
                    isValid = false;
                }
            });

            //if (!isValid) return false; //remove this

            // Step 1: Save form
            $.ajax({
                url: '/TRAM/Tram/TransferApply',
                type: 'POST',
                data: $("#msform").serialize(),
                success: function (response) {
                    if (!response.success) {
                        // Step 2: Fetch training on success of step 1
                        $.ajax({
                            url: '/TRAM/Tram/GetTrainingRecords',
                            type: 'GET',
                            success: function (data) {
                                $("#trainingContainer").html(data);
                                showNext(current_fs); // move to Step 2
                            },
                            error: function () {
                                alert("Failed to load training data.");
                            }
                        });
                    } else {
                        alert("Validation failed or error saving data.");
                    }
                },
                error: function () {
                    alert("Server error occurred.");
                }
            });

            return false;
        }
        // Step 2 onward: just move ahead
        showNext(current_fs);
    });

    $(".previous").click(function () {
        current_fs = $(this).closest("fieldset");
        previous_fs = current_fs.prev();
        $("#progressbar li").eq($("fieldset").index(current_fs)).removeClass("active");
        previous_fs.show();
        current_fs.animate({ opacity: 0 }, {
            step: function (now) {
                opacity = 1 - now;
                current_fs.css({ 'display': 'none', 'position': 'relative' });
                previous_fs.css({ 'opacity': opacity });
            },
            duration: 600
        });
    });

    function showNext(current_fs) {
        next_fs = current_fs.next();
        $("#progressbar li").eq($("fieldset").index(next_fs)).addClass("active");
        next_fs.show();
        current_fs.animate({ opacity: 0 }, {
            step: function (now) {
                opacity = 1 - now;
                current_fs.css({ 'display': 'none', 'position': 'relative' });
                next_fs.css({ 'opacity': opacity });
            },
            duration: 600
        });
    }
});
