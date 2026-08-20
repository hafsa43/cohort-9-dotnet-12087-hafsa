import { useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { taskService, categoryService } from '../services/services';

const statusBadge = (s) => {
  const map = { Pending: 'badge-pending', InProgress: 'badge-progress', Completed: 'badge-done' };
  return <span className={`badge ${map[s] || ''}`}>{s}</span>;
};
const priorityBadge = (p) => {
  const map = { High: 'badge-high', Medium: 'badge-medium', Low: 'badge-low' };
  return <span className={`badge ${map[p] || ''}`}>{p}</span>;
};

export default function TaskListPage() {
  const [tasks,      setTasks]      = useState([]);
  const [categories, setCategories] = useState([]);
  const [loading,    setLoading]    = useState(true);
  const [search,     setSearch]     = useState('');
  const [statusFilter,   setStatusFilter]   = useState('');
  const [priorityFilter, setPriorityFilter] = useState('');
  const [categoryFilter, setCategoryFilter] = useState('');
  const navigate = useNavigate();

  // Load categories for filter dropdown
  useEffect(() => {
    categoryService.getAll().then(setCategories).catch(console.error);
  }, []);

  // Fetch tasks with server-side filters
  useEffect(() => {
    setLoading(true);
    const params = {};
    if (search)         params.search     = search;
    if (statusFilter)   params.status     = statusFilter;
    if (priorityFilter) params.priority   = priorityFilter;
    if (categoryFilter) params.categoryId = categoryFilter;

    taskService.getAll(params)
      .then(setTasks)
      .catch(console.error)
      .finally(() => setLoading(false));
  }, [search, statusFilter, priorityFilter, categoryFilter]);

  const handleDelete = async (id, e) => {
    e.stopPropagation();
    if (!confirm('Delete this task?')) return;
    await taskService.remove(id);
    setTasks(t => t.filter(x => x.id !== id));
  };

  return (
    <div>
      <div className="page-header">
        <div>
          <h1 className="page-title">All Tasks</h1>
          <p className="page-subtitle">{tasks.length} task{tasks.length !== 1 ? 's' : ''} found</p>
        </div>
        <Link to="/tasks/new" className="btn btn-primary" id="new-task-btn">➕ New Task</Link>
      </div>

      {/* ── Server-side Filters ──────────────────────────────────────────── */}
      <div style={{ display: 'flex', gap: '0.75rem', marginBottom: '1.5rem', flexWrap: 'wrap' }}>
        <input
          id="search-tasks"
          className="form-input"
          placeholder="🔍 Search tasks..."
          value={search}
          onChange={e => setSearch(e.target.value)}
          style={{ maxWidth: 240 }}
        />
        <select id="status-filter" className="form-select" value={statusFilter}
          onChange={e => setStatusFilter(e.target.value)} style={{ maxWidth: 160 }}>
          <option value="">All Statuses</option>
          {['Pending', 'InProgress', 'Completed'].map(s => <option key={s} value={s}>{s}</option>)}
        </select>
        <select id="priority-filter" className="form-select" value={priorityFilter}
          onChange={e => setPriorityFilter(e.target.value)} style={{ maxWidth: 160 }}>
          <option value="">All Priorities</option>
          {['Low', 'Medium', 'High'].map(p => <option key={p} value={p}>{p}</option>)}
        </select>
        <select id="category-filter" className="form-select" value={categoryFilter}
          onChange={e => setCategoryFilter(e.target.value)} style={{ maxWidth: 180 }}>
          <option value="">All Categories</option>
          {categories.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
        </select>
        {(search || statusFilter || priorityFilter || categoryFilter) && (
          <button className="btn btn-ghost btn-sm" onClick={() => {
            setSearch(''); setStatusFilter(''); setPriorityFilter(''); setCategoryFilter('');
          }}>✕ Clear</button>
        )}
      </div>

      {/* ── Task Table ───────────────────────────────────────────────────── */}
      {loading ? <div className="spinner" /> : (
        <div className="table-wrap">
          <table>
            <thead>
              <tr>
                <th>#</th><th>Title</th><th>Status</th><th>Priority</th>
                <th>Category</th><th>Assigned To</th><th>Due Date</th><th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {tasks.length === 0 ? (
                <tr>
                  <td colSpan={8} style={{ textAlign: 'center', padding: '2.5rem', color: 'var(--text-muted)' }}>
                    No tasks found — try adjusting your filters
                  </td>
                </tr>
              ) : tasks.map(t => (
                <tr key={t.id} style={{ cursor: 'pointer' }} onClick={() => navigate(`/tasks/${t.id}`)}>
                  <td style={{ color: 'var(--text-muted)', fontFamily: 'monospace' }}>#{t.id}</td>
                  <td style={{ fontWeight: 600, color: 'var(--text-primary)' }}>{t.title}</td>
                  <td>{statusBadge(t.status)}</td>
                  <td>{priorityBadge(t.priority)}</td>
                  <td>{t.categoryName || '—'}</td>
                  <td>{t.assignedToUserName || '—'}</td>
                  <td>{t.dueDate ? new Date(t.dueDate).toLocaleDateString() : '—'}</td>
                  <td>
                    <div style={{ display: 'flex', gap: '0.4rem' }}>
                      <Link to={`/tasks/${t.id}/edit`} className="btn btn-ghost btn-sm"
                        onClick={e => e.stopPropagation()} id={`edit-${t.id}`}>✏️ Edit</Link>
                      <button className="btn btn-danger btn-sm"
                        onClick={(e) => handleDelete(t.id, e)} id={`delete-${t.id}`}>🗑️</button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}
