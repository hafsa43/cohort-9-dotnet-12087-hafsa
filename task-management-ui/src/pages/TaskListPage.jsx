import { useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { taskService } from '../services/services';

const statusBadge = (s) => {
  const map = { Pending: 'badge-pending', InProgress: 'badge-progress', Completed: 'badge-done' };
  return <span className={`badge ${map[s] || ''}`}>{s}</span>;
};
const priorityBadge = (p) => {
  const map = { High: 'badge-high', Medium: 'badge-medium', Low: 'badge-low' };
  return <span className={`badge ${map[p] || ''}`}>{p}</span>;
};

export default function TaskListPage() {
  const [tasks, setTasks] = useState([]);
  const [loading, setLoading] = useState(true);
  const [filter, setFilter] = useState('');
  const [statusFilter, setStatusFilter] = useState('All');
  const navigate = useNavigate();

  useEffect(() => {
    taskService.getAll()
      .then(setTasks)
      .catch(console.error)
      .finally(() => setLoading(false));
  }, []);

  const handleDelete = async (id, e) => {
    e.stopPropagation();
    if (!confirm('Delete this task?')) return;
    await taskService.remove(id);
    setTasks(t => t.filter(x => x.id !== id));
  };

  const filtered = tasks.filter(t =>
    (statusFilter === 'All' || t.status === statusFilter) &&
    t.title.toLowerCase().includes(filter.toLowerCase())
  );

  if (loading) return <div className="spinner" />;

  return (
    <div>
      <div className="page-header">
        <div>
          <h1 className="page-title">All Tasks</h1>
          <p className="page-subtitle">{filtered.length} task{filtered.length !== 1 ? 's' : ''} found</p>
        </div>
        <Link to="/tasks/new" className="btn btn-primary" id="new-task-btn">➕ New Task</Link>
      </div>

      {/* Filters */}
      <div style={{ display: 'flex', gap: '1rem', marginBottom: '1.5rem', flexWrap: 'wrap' }}>
        <input
          id="search-tasks"
          className="form-input"
          placeholder="🔍 Search tasks..."
          value={filter}
          onChange={e => setFilter(e.target.value)}
          style={{ maxWidth: 260 }}
        />
        <select id="status-filter" className="form-select" value={statusFilter} onChange={e => setStatusFilter(e.target.value)} style={{ maxWidth: 160 }}>
          {['All', 'Pending', 'InProgress', 'Completed'].map(s => <option key={s}>{s}</option>)}
        </select>
      </div>

      <div className="table-wrap">
        <table>
          <thead>
            <tr>
              <th>#</th><th>Title</th><th>Status</th><th>Priority</th>
              <th>Assigned To</th><th>Due Date</th><th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {filtered.length === 0 ? (
              <tr><td colSpan={7} style={{ textAlign: 'center', padding: '2rem', color: 'var(--text-muted)' }}>No tasks found</td></tr>
            ) : filtered.map(t => (
              <tr key={t.id} style={{ cursor: 'pointer' }} onClick={() => navigate(`/tasks/${t.id}`)}>
                <td style={{ color: 'var(--text-muted)', fontFamily: 'monospace' }}>#{t.id}</td>
                <td style={{ fontWeight: 600, color: 'var(--text-primary)' }}>{t.title}</td>
                <td>{statusBadge(t.status)}</td>
                <td>{priorityBadge(t.priority)}</td>
                <td>{t.assignedToUserName || '—'}</td>
                <td>{t.dueDate ? new Date(t.dueDate).toLocaleDateString() : '—'}</td>
                <td>
                  <div style={{ display: 'flex', gap: '0.4rem' }}>
                    <Link to={`/tasks/${t.id}/edit`} className="btn btn-ghost btn-sm" onClick={e => e.stopPropagation()} id={`edit-${t.id}`}>✏️ Edit</Link>
                    <button className="btn btn-danger btn-sm" onClick={(e) => handleDelete(t.id, e)} id={`delete-${t.id}`}>🗑️ Del</button>
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
