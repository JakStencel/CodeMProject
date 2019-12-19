/**
 * Authorization Roles
 */
const authRoles = {
    admin    : ['admin'],
    manager    : ['admin', 'manager'],
    developer     : ['admin', 'manager', 'developer'],
    onlyGuest: []
};

export default authRoles;
