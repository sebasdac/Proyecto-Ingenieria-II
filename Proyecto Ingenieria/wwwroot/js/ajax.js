const apiBaseUrl = "http://localhost:7277/api/Employee";


function fetchEmployeeById() {
    const employeeId = $("#employeeId").val();
    if (!employeeId) {
        alert("Por favor, ingresa un ID válido.");
        return;
    }

    $.ajax({
        url: `${apiBaseUrl}/${employeeId}`,
        type: "GET",
        success: function (employee) {
            renderEmployeeList([employee]);
        },
        error: function () {
            alert("No se encontró un empleado con ese ID.");
        }
    });
}


function fetchAllEmployees() {
    $.ajax({
        url: apiBaseUrl,
        type: "GET",
        success: function (employees) {
            renderEmployeeList(employees);
        },
        error: function () {
            alert("Error al devolver la lista de empleados.");
        }
    });
}


function renderEmployeeList(employees) {
    const $tbody = $("#employeeList tbody");
    $tbody.empty();

    employees.forEach(employee => {
        $tbody.append(`
                            <tr>
                                <td>${employee.id}</td>
                                <td>${employee.employeeName}</td>
                                <td>${employee.lastName}</td>
                                <td>${employee.position}</td>
                                <td>${employee.salary}</td>
                                <td>${new Date(employee.hireDate).toLocaleDateString()}</td>
                                <td>${employee.cedula}</td>
                                <td>${employee.phone}</td>
                                <td>${employee.email}</td>
                            </tr>
                        `);
    });
}
    </script >