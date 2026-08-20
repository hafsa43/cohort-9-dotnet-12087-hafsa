import { useEffect, useState } from 'react';
import { userService } from '../services/services';

export default function UsersPage() {
  const [users,   setUsers]   = useState([]);
  const [loading, setLoading] = useState(true);
  const [msg,     setMsg]     = useState('');

  useEffect(() => {
    userService.getAll()
      .then(setUsers)
      .catch(console.error)
      .finally(() => setLoading(false));
  }, []);

  const handleChangeRole = async (userId, currentRole) => {
    const newRole = currentRole === 'Admin' ? 'User' : 'Admin';
    if (!confirm(`Change role to ${newRole}?`)) return;
    try {
      await userService.changeRole({ userId, newRole });
      setUsers(us => us.map(u => u.id === userId ? { ...u, role: newRole } : u));
      setMsg(`Role updated to ${newRole}.`);
      setTimeout(() => setMsg(''), 3000);
    } catch (err) {
      setMsg(err.response?.data?.message || 'Failed to change role.');
    }
  };

  const handleDelete = async (userId) => {
    if (!confirm('Permanently delete this user and their data?')) return;
    try {
      await userService.remove(userId);
      setUsers(us => us.filter(u => u.id !== userId));
      setMsg('User deleted.');
      setTimeout(() => setMsg(''), 3000);
    } catch {
      setMsg('Failed to delete user.');
    }
  };

  if (loading) return <div className="spinner" />;

  return (
    <div>
      <div className="page-header">
        <div>
          <h1 className="page-title">User Management</h1>
          <p className="page-subtitle">{users.length} registered user{users.length !== 1 ? 's' : ''}</p>
        </div>
      </div>

      {msg && <div className={`alert ${msg.includes('Failed') ? 'alert-error' : 'alert-success'}`}>{msg}</div>}

      <div className="table-wrap">
        <table>
          <thead>
            <tr>
              <th>Name</th><th>Email</th><th>Role</th>
              <th>Tasks</th><th>Joined</th><th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {users.length === 0 ? (
              <tr><td colSpan={6} style={{ textAlign: 'center', padding: '2rem', color: 'var(--text-muted)' }}>No users found</td></tr>
            ) : users.map(u => (
              <tr key={u.id}>
                <td style={{ fontWeight: 600, color: 'var(--text-primary)' }}>
                  {u.firstName} {u.lastName}
                </td>
                <td>{u.email}</td>
                <td>
                  <span className={`badge ${u.role === 'Admin' ? 'badge-high' : 'badge-progress'}`}>
                    {u.role}
                  </span>
                </td>
                <td>{u.taskCount}</td>
                <td>{new Date(u.createdAt).toLocaleDateString()}</td>
                <td>
                  <div style={{ display: 'flex', gap: '0.4rem' }}>
                    <button
                      className="btn btn-ghost btn-sm"
                      onClick={() => handleChangeRole(u.id, u.role)}
                      id={`role-${u.id}`}
                      title="Toggle role"
                    >
                      🔄 {u.role === 'Admin' ? '→ User' : '→ Admin'}
                    </button>
                    <button
                      className="btn btn-danger btn-sm"
                      onClick={() => handleDelete(u.id)}
                      id={`delete-user-${u.id}`}
                    >
                      🗑️
                    </button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}
