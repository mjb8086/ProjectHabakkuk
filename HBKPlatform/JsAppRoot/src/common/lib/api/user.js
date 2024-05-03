
export async function getCurrentUserData()
{
    let data = {};
    let error = {}; // Todo: Handle error condition with toast?
    
    await fetch('/api/common/getuserdata')
        .then((res) => res.json())
        .then((json) => (data = json))
        .catch((err) => (error = err));
    
    return data;
}