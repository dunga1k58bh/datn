function copyToClipboard() {
    var inputField = document.getElementById('Client_ClientId');
    inputField.select();
    document.execCommand('copy');

    // Show the success message
    var successAlert = document.getElementById('copySuccessAlert');
    successAlert.style.display = 'block';

    // Hide the success message after a few seconds (e.g., 3 seconds)
    setTimeout(function () {
        successAlert.style.display = 'none';
    }, 3000);
}

function addRedirectField() {
    var container = document.getElementById('redirectUriContainer');
    var newIndex = container.children.length;

    var inputGroup = document.createElement('div');
    inputGroup.className = 'input-group mt-1';

    var input = document.createElement('input');
    input.type = 'text';
    input.className = 'form-control';
    input.placeholder = 'Enter Redirect URI';
    input.name = 'RedirectUris[' + newIndex + ']';
    input.value = 'https://'

    var buttonAppend = document.createElement('div');
    buttonAppend.className = 'input-group-append';

    var removeButton = document.createElement('button');
    removeButton.type = 'button';
    removeButton.className = 'btn btn-danger';
    removeButton.innerHTML = 'Remove';
    removeButton.addEventListener('click', function() {
        removeRedirectField(newIndex);
    });

    buttonAppend.appendChild(removeButton);
    inputGroup.appendChild(input);
    inputGroup.appendChild(buttonAppend);

    container.appendChild(inputGroup);
}

function removeRedirectField(index) {
    var container = document.getElementById('redirectUriContainer');
    var removedElement = container.children[index];
    container.removeChild(removedElement);
}

function generateClientSecret(id) {
    // AJAX post to the server using jQuery
    $.ajax({
        url: '/clients/generate-secret',
        type: 'POST',
        dataType: 'json',
        data: { ClientId: parseInt(id) }, // Use "id" as the property name
        success: function (data) {

            console.log(data);
            // Open a popup form (you can replace this with your own logic)
            openPopupForm(data.clientSecret);
        },
        error: function (error) {
            console.error('Error:', error);
        }
    });

        function openPopupForm(code) {
        // Replace this with your logic to open a popup form
        alert('New Secret: ' + code);
    }
}