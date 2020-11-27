$(function () {
    $('.editModal').modal();
});

function EnrollStudentToCourse(studentId) {
    $.ajax({
        url: '/Admin/Course/EnrollStudentToClass' + productId, // The method name + paramater
        success: function (data) {
            $('#modalWrapper').html(data); // This should be an empty div where you can inject your new html (the partial view)
        }
    });
}